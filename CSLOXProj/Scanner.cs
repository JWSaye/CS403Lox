using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLOXProj
{
    public class Scanner
    {
        private readonly String source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(String source)
        {
            this.source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
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
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else
                    {
                        addToken(TokenType.SLASH);
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
            return source[current++];
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, Object literal)
        {
            string text = source.Substring(start, current-start);
            tokens.Add(new Token(type, text, literal, line));
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source[current];
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
            string value = source.Substring(start + 1, current - 1 - (start + 1));
            addToken(TokenType.STRING, value);
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

            addToken(TokenType.NUMBER,
                Double.Parse(source.Substring(start, current-start)));
        }

        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private void identifier()
        {
            while (isAlphaNumeric(peek())) advance();

            String text = source.Substring(start, current-start);
            TokenType type;
            if (!keywords.TryGetValue(text, out type)) type = TokenType.IDENTIFIER;
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

        private static readonly Dictionary<string, TokenType> keywords;

        static Scanner(){
            keywords = new Dictionary<string, TokenType>();
            keywords.Add("and",    TokenType.AND);
            keywords.Add("class",  TokenType.CLASS);
            keywords.Add("else",   TokenType.ELSE);
            keywords.Add("false",  TokenType.FALSE);
            keywords.Add("for",    TokenType.FOR);
            keywords.Add("fun",    TokenType.FUN);
            keywords.Add("if",     TokenType.IF);
            keywords.Add("nil",    TokenType.NIL);
            keywords.Add("or",     TokenType.OR);
            keywords.Add("print",  TokenType.PRINT);
            keywords.Add("return", TokenType.RETURN);
            keywords.Add("super",  TokenType.SUPER);
            keywords.Add("this",   TokenType.THIS);
            keywords.Add("true",   TokenType.TRUE);
            keywords.Add("var",    TokenType.VAR);
            keywords.Add("while",  TokenType.WHILE);
       }
}
}
