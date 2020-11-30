using System.Globalization;
using System.IO;
using System.Text;

namespace Calculator
{
    public class Tokenizer
    {
        public Tokenizer(TextReader reader)
        {
            _reader = reader;
            NextChar();
            NextToken();
        }

        TextReader _reader;
        char _currentChar;
        Token _currentToken;
        double _number;
        string _identifier;

        public Token Token
        {
            get { return _currentToken; }
        }

        public double Number
        {
            get { return _number; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        void NextChar()
        {
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        public void NextToken()
        {
            // Пропуск пробелов
            while (char.IsWhiteSpace(_currentChar))
            {
                NextChar();
            }

            switch (_currentChar)
            {
                case '\0':
                    _currentToken = Token.EOF;
                    return;

                case '+':
                    NextChar();
                    _currentToken = Token.Add;
                    return;

                case '-':
                    NextChar();
                    _currentToken = Token.Subtract;
                    return;

                case '*':
                    NextChar();
                    _currentToken = Token.Multiply;
                    return;

                case '/':
                    NextChar();
                    _currentToken = Token.Divide;
                    return;

                case '(':
                    NextChar();
                    _currentToken = Token.OpenParens;
                    return;

                case ')':
                    NextChar();
                    _currentToken = Token.CloseParens;
                    return;

                case ',':
                    NextChar();
                    _currentToken = Token.Comma;
                    return;
                case ';':
                    NextChar();
                    _currentToken = Token.Semicolon;
                    return;
                case '=':
                    NextChar();
                    _currentToken = Token.Equal;
                    return;
            }

            // Число?
            if (char.IsDigit(_currentChar) || _currentChar =='.')
            {
                // Захват целого или с плавающей точкой
                var sb = new StringBuilder();
                bool haveDecimalPoint = false;
                while (char.IsDigit(_currentChar) || (!haveDecimalPoint && _currentChar == '.'))
                {
                    sb.Append(_currentChar);
                    haveDecimalPoint = _currentChar == '.';
                    NextChar();
                }
                _number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                _currentToken = Token.Number;
                return;
            }

            // Идентификатор начинается с символа или ниж подчёркивания
            if (char.IsLetter(_currentChar) || _currentChar == '_')
            {
                var sb = new StringBuilder();

                while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Установка токена
                _identifier = sb.ToString();
                _currentToken = Token.Identifier;
                return;
            }
        }
    }
}
