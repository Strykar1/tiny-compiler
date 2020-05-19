using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using TinyCompiler.Model;


namespace TinyCompiler.Controller
{
    class Parser
    {
        private List<Token> tokens ;
        public String ErrorString { get; private set; }

        int currentTokenIndex;
        Token currentToken = new Token();

        public Parser(List<Token> tokenList)
        {
            this.tokens = tokenList;
        }
        public TreeNode Parse()
        {
            currentTokenIndex = 0;
            currentToken = tokens[currentTokenIndex];
            ErrorString = "";

            return statementSequence(); ;
        }
        private void setNextToken()
        {
            if (currentTokenIndex < tokens.Count-1)
            {
                currentTokenIndex++;
                currentToken = tokens[currentTokenIndex];                
            }

        }
        private Token getLastToken()
        {
            return tokens[currentTokenIndex - 1];
        }
        private void ThrowSyntaxError(TokenType expectedTokenType,TokenType foundTokenType)
        {
            ErrorString = ("Syntax Error: Expected token type ( "+expectedTokenType.ToString()+" ) and found ( "+ foundTokenType.ToString() + " )");
        }
        private void ThrowInvalidSyntaxError(TokenType foundTokenType)
        {
            ErrorString = ("Syntax Error: found Invalid TokenType ( " + foundTokenType.ToString() + " )");
        }
        private void match(TokenType expectedTokenType)
        {
            if (currentToken.Type == expectedTokenType)
            {
                setNextToken();
            }
            else
            {
                ThrowSyntaxError(expectedTokenType, currentToken.Type);
            }
        }
        TreeNode statementSequence()
        {
            TreeNode start = new TreeNode();
            TreeNode treeNode = new TreeNode();

            treeNode = statement();
            start.Nodes.Add(treeNode);
            while (currentToken.Type != TokenType.EndOfFile &&
                    currentToken.Type != TokenType.End &&
                    currentToken.Type != TokenType.Until &&
                    currentToken.Type != TokenType.Else) { 
                match(TokenType.SemiColon);
                TreeNode nextTempNode = new TreeNode();
                nextTempNode = statement();

                Console.WriteLine(currentToken.Type.ToString() + " is token number: " + currentTokenIndex + " of " + tokens.Count);
                start.Nodes.Add(nextTempNode);
            }
            return start;
        }
        TreeNode statement()
        {
            TreeNode treeNode = new TreeNode();
            switch (currentToken.Type)
            {
                case TokenType.Id:
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
                default:
                    ThrowInvalidSyntaxError(currentToken.Type);
                    break;
            }
            return treeNode;
        }
        TreeNode ifStatement()
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "if";

            match(TokenType.If);
            treeNode.Nodes.Add(expression());
            match(TokenType.Then);
            treeNode.Nodes.Add(statementSequence());
            if(currentToken.Type == TokenType.Else)
            {
                match(TokenType.Else);
                treeNode.Nodes.Add(statementSequence());
            }
            match(TokenType.End);

            return treeNode;
        }
        TreeNode repeatStatement()
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "repeat";
            match(TokenType.Repeat);
            treeNode.Nodes.Add(statementSequence());
            match(TokenType.Until);
            treeNode.Nodes.Add(expression());

            return treeNode;
        }
        TreeNode assignStatement()
        {
            TreeNode treeNode = new TreeNode();
            
            match(TokenType.Id);
            treeNode.Text = "assign \n("+getLastToken().Lexeme+")";
            match(TokenType.Assign);
            treeNode.Nodes.Add(expression());

            return treeNode;
        }

        TreeNode readStatement()
        {
            TreeNode treeNode = new TreeNode();

            match(TokenType.Read);
            match(TokenType.Id);

            treeNode.Text = "read \n("+getLastToken().Lexeme+")";
            return treeNode;
        }
        TreeNode writeStatement()
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "write";
            match(TokenType.Write);
            treeNode.Nodes.Add(expression());

            return treeNode;
        }
        TreeNode expression()
        {
            TreeNode treeNode = new TreeNode();
            TreeNode nextTempNode = new TreeNode();

            treeNode = simpleExpression();
            if (currentToken.Type == TokenType.LessThan || currentToken.Type == TokenType.IsEqual)
            {
                nextTempNode.Text = "op \n("+currentToken.Lexeme+")";
                nextTempNode.Nodes.Add(treeNode);
                match(currentToken.Type);
                nextTempNode.Nodes.Add(simpleExpression());

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
                nextTempNode.Nodes.Add(treeNode);
                match(currentToken.Type);
                nextTempNode.Text = "op\n(" + getLastToken().Lexeme + ")";
                nextTempNode.Nodes.Add(term());

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
                nextTempNode.Nodes.Add(treeNode);
                match(currentToken.Type);
                nextTempNode.Text = "op\n(" + getLastToken().Lexeme + ")";
                nextTempNode.Nodes.Add(factor());

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
                    treeNode.Text = "Integer\n("+currentToken.Lexeme+")";
                    match(TokenType.Integer);
                    break;
                case TokenType.Float:
                    treeNode.Text = "Float\n(" + currentToken.Lexeme + ")";
                    match(TokenType.Float);
                    break;
                case TokenType.Id:
                    treeNode.Text = "Id\n("+currentToken.Lexeme+")";
                    match(TokenType.Id);
                    break;
            }

            return treeNode;
        }
    }
}
