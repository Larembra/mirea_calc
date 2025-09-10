using System;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


double display = 0.0;
double memory  = 0.0;

Console.WriteLine("Classic simplified calculator, type 'exit' to finish work");
Console.WriteLine("Operations: + - * / %  1/x  x^2  sqrt  M+  M-  MR  MC");
Console.WriteLine("Examples:\n2 + 2\n9 sqrt\nMR + 5\n\n");

bool running = true;

while (running)
{
    
    var line = Console.ReadLine();
    if (line == null) break;

    line = line.Trim();
    if (line.Length == 0) continue;


    if (line == "exit") {
        break;
    }

    if (line == "MC")
    {
        memory = 0;
        Console.WriteLine("Memory cleared (M = 0).");
        continue;
    }

    if (line == "MR")
    {
        display = memory;
        Console.WriteLine($"= {display}");
        continue;
    }

    var parts = line.Split(' ');
    if (parts.Length != 2 && parts.Length != 3)
    {
        Console.WriteLine("Format error: line must contain only \"exit\", \"MS\", \"MR\" or 2 or 3 tokens. Examples: '5 M+' or '2 + 2' or 'MR'.");
        continue;
    }

    bool TryParseNumberToken(string token, out double value)
    {
        value = 0;

        if (token == "MR")
        {
            value = memory;
            return true;
        }

        if (token.Length > 10)
        {
            Console.WriteLine($"Error: numeric token '{token}' exceeds 10 characters.");
            return false;
        }

        if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            Console.WriteLine($"Error: token '{token}' is not a valid number.");
            return false;
        }
        return true;
    }

    bool IsBinary(string s) => s is "+" or "-" or "*" or "/" or "%";
    bool IsUnary(string s) => s is "1/x" or "x^2" or "sqrt";
    bool IsMemoryOp(string s) => s is "M+" or "M-";

    // 3-token form: A OP B
    if (parts.Length == 3)
    {
        string leftToken  = parts[0];
        string opToken    = parts[1];
        string rightToken = parts[2];

        if (!IsBinary(opToken))
        {
            Console.WriteLine("Error: in a 3-token line, the second token must be a binary operator (+ - * / %).");
            continue;
        }

        if (!TryParseNumberToken(leftToken, out double left))  continue;
        if (!TryParseNumberToken(rightToken, out double right)) continue;

        double result;
        switch (opToken)
        {
            case "+": result = left + right; break;
            case "-": result = left - right; break;
            case "*": result = left * right; break;
            case "/":
                if (right == 0)
                {
                    Console.WriteLine("Error: division by zero.");
                    continue;
                }
                result = left / right;
                break;
            case "%":
                if (right == 0)
                {
                    Console.WriteLine("Error: modulus by zero.");
                    continue;
                }
                result = left % right;
                break;
            default:
                Console.WriteLine("Internal error: unknown binary operator.");
                continue;
        }

        display = result;
        if (double.IsInfinity(display))
            Console.WriteLine("Result: Infinity (overflow).");
        else if (double.IsNaN(display))
            Console.WriteLine("Result: NaN (invalid operation).");
        else
            Console.WriteLine($"= {display}");

        continue;
    }

    
    {
        string numberToken = parts[0];
        string opToken     = parts[1];

        if (!TryParseNumberToken(numberToken, out double operand))
            continue;

        if (IsUnary(opToken))
        {
            switch (opToken)
            {
                case "1/x":
                    if (operand == 0)
                    {
                        Console.WriteLine("Error: division by zero in 1/x.");
                        continue;
                    }
                    display = 1.0 / operand;
                    break;

                case "x^2":
                    display = operand * operand;
                    break;

                case "sqrt":
                    if (operand < 0)
                    {
                        Console.WriteLine("Error: square root of a negative number.");
                        continue;
                    }
                    display = Math.Sqrt(operand);
                    break;
            }

            if (double.IsInfinity(display))
                Console.WriteLine("Result: Infinity (overflow).");
            else if (double.IsNaN(display))
                Console.WriteLine("Result: NaN (invalid operation).");
            else
                Console.WriteLine($"= {display}");
        }
        else if (IsMemoryOp(opToken))
        {
            switch (opToken)
            {
                case "M+":
                    memory += operand;
                    Console.WriteLine($"M = {memory}");
                    break;
                case "M-":
                    memory -= operand;
                    Console.WriteLine($"M = {memory}");
                    break;
            }
            
        }
        else
        {
            Console.WriteLine("Error: in a 2-token line the second token must be a unary op (1/x, x^2, sqrt) or memory op (M+, M-).");
        }
    }
}
