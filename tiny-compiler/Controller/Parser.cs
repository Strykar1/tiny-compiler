using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using TinyCompiler.Model;


namespace TinyCompiler.Controller
{
    public class Parser
    {
        private List<Token> tokens ;

        int currentTokenIndex;
        Token currentToken = new Token();

        Parser(List<Token> tokenList)
        {
            this.tokens = tokenList;
        }
        public TreeNode Parse()
        {
            currentTokenIndex = 0;
            currentToken = tokens[currentTokenIndex];

            return statementSequence(); ;
        }
        private void setNextToken()
        {
            if (tokens.ElementAtOrDefault(currentTokenIndex) != null)
            {
                currentToken = tokens[currentTokenIndex];
                currentTokenIndex++;
            }
        }
        private Token getLastToken()
        {
            return tokens[currentTokenIndex - 1];
        }
        private void ThrowSyntaxError()
        {
            //todo syntax error
        }
        private void match(TokenType expectedTokenType)
        {
            if (currentToken.Type == expectedTokenType)
                setNextToken();
            else
            {
                ThrowSyntaxError();
            }
        }
        TreeNode statementSequence()
        {
            TreeNode start = new TreeNode();
            TreeNode treeNode = new TreeNode();
            TreeNode nextTempNode = new TreeNode();

            treeNode = statement();
            start = treeNode;
            while (currentToken.Type == Token.SPECIAL_SYMBOLS[";"])
            {
                match(TokenType.SemiColon);
                nextTempNode = statement();
                start.Nodes.Insert(0, treeNode);
                start.Nodes.Insert(1,nextTempNode);
                treeNode = nextTempNode;
            }
            return start;
        }
        TreeNode statement()
        {
            TreeNode treeNode = new TreeNode();
            switch (currentToken.Type)
            {
                case TokenType.Assign:
                    treeNode = assignStatement();
                    break;
                case TokenType.If:
                    treeNode = ifStatement();
                    break;
                case TokenType.Repeat:
                    treeNode = repeatStatement();
                    break;
                case TokenType.Read:
                    treeNode = readStatement();
                    break;
                case TokenType.Write:
                    treeNode = writeStatement();
                    break;
            }
            return treeNode;
        }
        TreeNode ifStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.If);
            treeNode.Nodes.Insert(0,expression());
            match(TokenType.Then);
            treeNode.Nodes.Insert(1, statementSequence());
            if(currentToken.Type == TokenType.Else)
            {
                match(TokenType.Else);
                treeNode.Nodes.Insert(2, statementSequence());
            }
            match(TokenType.End);

            return treeNode;
        }
        TreeNode repeatStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.Repeat);
            treeNode.Nodes.Insert(0,statementSequence());
            match(TokenType.Until);
            treeNode.Nodes.Insert(0, expression());

            return treeNode;
        }
        TreeNode assignStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.Id);
            match(TokenType.Assign);
            treeNode.Nodes.Insert(0, expression());

            return treeNode;
        }

        TreeNode readStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.Read);
            match(TokenType.Id);

            treeNode.Text = getLastToken().Lexeme;
            return treeNode;
        }
        TreeNode writeStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.Write);
            treeNode.Nodes.Insert(0, expression());

            return treeNode;
        }
        TreeNode expression()
        {
            TreeNode treeNode = new TreeNode();
            TreeNode nextTempNode = new TreeNode();

            treeNode = simpleExpression();
            while (currentToken.Type == TokenType.LessThan || currentToken.Type == TokenType.IsEqual)
            {
                nextTempNode.Nodes.Insert(0, treeNode);
                match(currentToken.Type);
                nextTempNode.Nodes.Insert(1, simpleExpression());

                treeNode = nextTempNode;
            }

            return treeNode;
        }
        TreeNode simpleExpression()
        {
            TreeNode treeNode = new TreeNode();
            TreeNode nextTempNode = new TreeNode();

            treeNode = term();
            while (currentToken.Type == TokenType.Plus || currentToken.Type == TokenType.Minus)
            {
                nextTempNode.Nodes.Insert(0, treeNode);
                match(currentToken.Type);
                nextTempNode.Nodes.Insert(1, term());

                treeNode = nextTempNode;
            }
            return treeNode;
        }
        TreeNode term()
        {
            TreeNode treeNode = new TreeNode();
            TreeNode nextTempNode = new TreeNode();

            treeNode = factor();
            while (currentToken.Type == TokenType.Mult || currentToken.Type == TokenType.Division)
            {
                nextTempNode.Nodes.Insert(0, treeNode);
                match(currentToken.Type);
                nextTempNode.Nodes.Insert(1, factor());

                treeNode = nextTempNode;
            }
            return treeNode;
        }
        TreeNode factor()
        {
            TreeNode treeNode = new TreeNode();
            switch (currentToken.Type)
            {
                case TokenType.BraceLeft:
                    match(TokenType.BraceLeft);
                    treeNode = expression();
                    match(TokenType.BraceRight);
                    break;
                case TokenType.Integer:
                    treeNode.Text = currentToken.Lexeme;
                    match(TokenType.Integer);
                    break;
                case TokenType.Float:
                    treeNode.Text = currentToken.Lexeme;
                    match(TokenType.Float);
                    break;
                case TokenType.Id:
                    treeNode.Text = currentToken.Lexeme;
                    match(TokenType.Id);
                    break;
            }

            return treeNode;
        }
    }
}
