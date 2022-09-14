#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <any>
#include "TokenType.cpp"

using namespace std;

class Token{
    static TokenType type;
    static string lexeme;
    static any literal;
    static int line;
    
    public:
        Token(TokenType type, string lexeme, any literal, int line){
            this->type = type;
            this->lexeme = lexeme;
            this->literal = literal;
            this->line = line;
        }

        string toString(){
            return type + " " + lexeme + " " + any_cast<string>(literal);
        }
};