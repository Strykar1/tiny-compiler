﻿using System;
using TinyCompiler.Model;

namespace TinyCompiler.Controller
{
    class Scanner
    {

        Token current_token = new Token();
        private Char SavedChar = null;

        public Token getToken()
        {
            string lexeme = "";
            
            TokenType type = TokenType.EndOfFile;
            State current_state = State.Start;
            while (current_state != State.Done)
            {
                char ch;
                if(SavedChar != null){
                    ch = SavedChar;
                    SavedChar = null;
                }else
                    ch = getNextChar();
                switch (current_state)
                {
                    case State.Start:
                        Boolean success;
                        current_state = getNewState(ch);
                        break;
                    case State.InSlash:
                        if(ch == '*')
                            current_state = State.InComment;
                        else
                        {
                            current_token.Type = TokenType.Division;
                            current_state = State.Done;
                            SavedChar = ch;
                        }
                        break;
                    case State.InComment:
                        if(ch == '*')
                            current_state = State.EndingComment;
                        break;
                    case State.EndingComment:
                        if(ch == '*')
                            continue;
                        else if(ch == '/')
                        {
                            current_state = State.Done;
                        }
                        else
                            current_state = State.InComment;
                        break;
                    case State.Identifier:
                        if(char.IsLetterOrDigit(ch))
                        {
                            lexeme += ch;
                            continue;
                        }
                        else{
                            current_state = State.Done;
                            SavedChar = ch;
                        }
                        break;
                    case State.Int:
                        if(char.IsDigit(ch))
                        {
                            lexeme+=ch;
                        }
                        else if(ch == '.')
                        {
                            lexeme+=ch;
                            current_state = State.Float;
                        }
                        else{
                            current_state = State.Done;
                            SavedChar = ch;
                        }
                        break;
                    case State.Float:
                        if(Char.IsDigit(ch))
                            continue;
                        else{
                            current_state = State.Done;
                            SavedChar = ch;
                        }
                        break;
                    case State.String:
                        current_token = getStringToken();
                        current_state = State.Done;
                        break;

                    case State.Assignment:
                        current_state = State.Done;
                        if(ch == '=')
                        {
                            current_token.Type = TokenType.Assign;
                        }
                        else
                        {
                            //todo error
                        }
                        break;

                    case State.InAnd:
                        current_state = State.Done;
                        if (ch == '&')
                        {
                            current_token.Type = TokenType.BoolAnd;
                        }
                        else
                        {
                            //todo error
                        }
                        break;

                    case State.InNotEqual:
                        current_state = State.Done;
                        if (ch == '>')
                        {
                            current_token.Type = TokenType.IsNotEqual;
                        }
                        else
                        {
                            //todo error
                        }
                        break;

                    case State.InOR:
                        current_state = State.Done;
                        if (ch == '|')
                        {
                            current_token.Type = TokenType.BoolOR;
                        }
                        else
                        {
                            //todo error
                        }
                        break;
                }
            }
   

            current_token.Lexeme = lexeme;
            current_token.Type = type;
            if(current_token.Type == TokenType.Id)
            {
                setTypeIfReserved();
            }
            return current_token;
        }

        private char getNextChar()
        {
            throw new NotImplementedException();
        }

        private Token getSingleToken()
        {
            throw new NotImplementedException();
        }

        private State getNewState(Char c)
        {
            State state = State.Error;
            switch(c){
                case '/':
                    state = State.InSlash;
                    break;
                case '<':
                    state = State.InNotEqual;
                    break;
                case '&':
                    state = State.InAnd;
                    break;
                case '|':
                    state = State.InOR;
                    break;
                case ':':
                    state = State.Assignment;
                    break;
                case '"':
                    state = State.String;
                    break;
                case default:
                    if(Char.IsLetter(c))
                        state = State.Identifier;
                    else if(Char.IsDigit(c))
                        state = State.Int;
                    else if(c == ' ' || c == '\t' || c == '\n'){ //TODO check if it catches all white space conditions
                        state = State.Start;
                    }
                    else
                        foreach(string word in Token.SPECIAL_SYMBOLS.Keys)
                        {
                            if (c.Equals(word))
                            {
                                state = State.Done;
                                current_token.Type = Token.SPECIAL_SYMBOLS[word];
                            }
                        }
                    break;
            }
            return state;
        }

        private void setTypeIfReserved()
        {
            foreach(string word in Token.RESERVED_WORDS.Keys)
            {
                if (current_token.Lexeme.Equals(word))
                {
                    current_token.Type = Token.RESERVED_WORDS[word];
                }
            }
        }
    }
}
