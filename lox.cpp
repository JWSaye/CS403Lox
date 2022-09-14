#include "Scanner.cpp"
using namespace std;

class Lox{
    bool hadError = false;

    private:
        void runFile(string path){
            ifstream myfile(path);
            string file;
            myfile >> file;
            run(file);

            if(hadError) exit(65);
        }

        void runPrompt(){
            for(;;){
                string line;
                cout << "> ";
                getline(cin, line);
                if(!cin) break;
                run(line);

                hadError = false;
            }
        }

        void run(string source){
            Scanner* scanner = new Scanner(source);
            vector<Token> tokens = scanner->scanTokens();

            for(Token token : tokens){
                cout << token.toString() << endl;
            }
        }

        void report(int line, string where, string message){
            cout << "[line " << line << "] Error" << where << ": " << message << endl;
            hadError = true;
        }

    public:
        int main(int argc, char** argv){
            if(argc > 1){
                cout << "Usage: cpplox [script]" << endl;
                exit(64);
            }

            else if(argc == 1){
                runFile(*argv);
            }

            else{
                runPrompt();
            }
        }

        void error(int line, string message){
            report(line, "", message);
        }
};