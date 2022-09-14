#include <map>
#include "Token.cpp"

using namespace std;

class Scanner {
    private:
        static std::string source;
        const vector<Token*> tokens;
        int start = 0;
        int current = 0;
        int line = 1;

        bool isAtEnd() {
            return current >= source.length();
        }

        char advance() {
            return char((source.substr(current, 1)).c_str());
        }

        void addToken(TokenType type) {
            addToken(type, nullptr);
        }

        void addToken(TokenType type, any literal) {
            std::string text = source.substr(start, current-start);
            tokens.push_back(new Token(type, text, literal, line));
        }

        bool match(char expected) {
            if(isAtEnd()) return false;
            if(source[current] != expected) return false;

            current++;
            return true;
        }

        char peek(){
            if(isAtEnd()) return '\0';
            return source[current];
        }

        char peekNext(){
            if(current + 1 >= source.length()) return '\0';
            return source[current + 1];
        }

        void string(){
            while(peek() != '"' && !isAtEnd()){
                if(peek() == '\n') line++;
                advance();
            }

            if(isAtEnd()){
                lox.lox.error(line, "Unterminated std::string.");
                return;
            }

            advance();

            std::string value = source.substr(start + 1, current - 1);
            addToken(STRING, value);
        }

        bool isDigit(char c){
            return c >= '0' && c <= '9';
        }

        void number(){
            while(isDigit(peek())) advance();

            if(peek() == '.' && isDigit(peekNext())){
                advance();

                while(isDigit(peek())) advance();
            }

            addToken(NUMBER, stod(source.substr(start, current)));
        }

        void scanToken() {
            char c = advance();
            switch (c) {
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
                    if(match('/')){
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }

                    else{
                        addToken(SLASH);
                    }

                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"': std::string();

                default:
                    if(isDigit(c)){
                        number();
                    }

                    else if(isAlpha(c)){
                        identifier();
                    }
                    
                    else{
                        lox.error(line, "Unexpected character.");
                    }

                    break;
            }
        }

        void identifier(){
            while(isAlphaNumeric(peek())) advance();

            std::string text = source.substr(start, current);
            TokenType type;
            map<std::string, TokenType>::iterator it = keywords.find(text);
            if(it == keywords.end()) type = IDENTIFIER;
            else type = it->second;
            addToken(type);
        }

        bool isAlpha(char c){
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        bool isAlphaNumeric(char c){
            return isAlpha(c) || isDigit(c);
        }

        static map<std::string, TokenType> keywords;
/*
        static{
            keywords = new map<>();
            keywords.put("and",     AND);
            keywords.put("class",   CLASS);
            keywords.put("else",    ELSE);
            keywords.put("false",   FALSE);
            keywords.put("for",     FOR);
            keywords.put("fun",     FUN);
            keywords.put("if",      IF);
            keywords.put("nil",     NIL);
            keywords.put("or",      OR);
            keywords.put("print",   PRINT);
            keywords.put("return",  RETURN);
            keywords.put("super",   SUPER);
            keywords.put("this",    THIS);
            keywords.put("true",    TRUE);
            keywords.put("var",     VAR);
            keywords.put("while",   WHILE);
        };*/
    public:
        Scanner(std::string source){
            this->source = source;
        }

        vector<Token> scanTokens() {
            while(!isAtEnd()) {
                start = current;
                scanToken();
            }

            tokens.push_back(new Token(EF, "", nullptr, line));
            return tokens;
        }
};