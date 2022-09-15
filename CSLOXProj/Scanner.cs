﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLOXProj
{
    class Scanner
    {
        private readonly String source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        Scanner(String source)
        {
            this.source = source;
        }

        List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                scanToken();
            }

            tokens.push_back(new Token(EOF, "", null, line));
            return tokens;
        }

        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        private void scanToken()
        {
            char c = advance();
            switch (c)
            {
                case '(': addToken(LEFT_PAREN); break;
                case ')': addToken(RIGHT_PAREN); break;
                case '{': addToken(LEFT_BRACE); break;
                case '}': addToken(RIGHT_BRACE); break;
                case ',': addToken(COMMA); break;
                case '.': addToken(DOT); break;
                case '-': addToken(MINUS); break;
                case '+': addToken(PLUS); break;
                case ';': addToken(SEMICOLON); break;
                case '*': addToken(STAR); break;
                case '!':
                    addToken(match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    addToken(match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    addToken(match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else
                    {
                        addToken(SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    line++;
                    break;
                case '"': String(); break;

                default:
                    if (isDigit(c))
                    {
                        number();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();

                    }
                    else
                    {
                        Lox.error(line, "Unexpected character.");
                    }            
                    break;
            }
        }

        private char advance()
        {
            return source.charAt(current++);
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, Object literal)
        {
            String text = source.substring(start, current);
            tokens.add(new Token(type, text, literal, line));
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source.charAt(current) != expected) return false;

            current++;
            return true;
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source.charAt(current);
        }

        private void String () 
        {
            while (peek() != '"' && !isAtEnd()) {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd()) {
                Lox.error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            advance();

            // Trim the surrounding quotes.
            string value = source.substring(start + 1, current - 1);
            addToken(STRING, value);
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void number()
        {
            while (isDigit(peek())) advance();

            // Look for a fractional part.
            if (peek() == '.' && isDigit(peekNext()))
            {
                // Consume the "."
                advance();

                while (isDigit(peek())) advance();
            }

            addToken(NUMBER,
                Double.parseDouble(source.substring(start, current)));
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source.charAt(current + 1);
        }

        private void identifier()
        {
            while (isAlphaNumeric(peek())) advance();

            String text = source.substring(start, current);
            TokenType type = keywords.get(text);
            if (type == null) type = IDENTIFIER;
            addToken(type);
        }

        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        private static readonly Map<String, TokenType> keywords;

        static {
            keywords = new HashMap<>();
            keywords.put("and",    AND);
            keywords.put("class",  CLASS);
            keywords.put("else",   ELSE);
            keywords.put("false",  FALSE);
            keywords.put("for",    FOR);
            keywords.put("fun",    FUN);
            keywords.put("if",     IF);
            keywords.put("nil",    NIL);
            keywords.put("or",     OR);
            keywords.put("print",  PRINT);
            keywords.put("return", RETURN);
            keywords.put("super",  SUPER);
            keywords.put("this",   THIS);
            keywords.put("true",   TRUE);
            keywords.put("var",    VAR);
            keywords.put("while",  WHILE);
       }
}
}