﻿using System.Text;

namespace Holo;

public static class Parser {
    public static List<Statement> Parse(string Code) {
        List<Statement> Statements = [];

        for (int Index = 0; Index < Code.Length; Index++) {
            // Whitespace
            if (char.IsWhiteSpace(Code[Index])) {
                continue;
            }
            // Identifier
            if (EatIdentifier(Code, ref Index) is string Identifier) {
                EatSpaces(Code, ref Index);

                // Method call (no arguments or brackets)
                if (EatLineBreak(Code, ref Index)) {
                    Statements.Add(new CallStatement(new SelfExpression(), Identifier, []));
                    continue;
                }
                // Method call (one argument)
                else if (EatExpression(Code, ref Index) is Expression Argument) {
                    Statements.Add(new CallStatement(new SelfExpression(), Identifier, [Argument]));
                    continue;
                }
                // Invalid
                else {
                    throw new Exception($"invalid character: '{Code[Index]}'");
                }
            }
            // Invalid
            throw new Exception($"invalid character: '{Code[Index]}'");
        }

        return Statements;
    }

    private static void EatSpaces(string Code, ref int Index) {
        for (; Index < Code.Length; Index++) {
            if (Code[Index] is '\n' or '\r' || !char.IsWhiteSpace(Code[Index])) {
                break;
            }
        }
    }
    private static bool EatLineBreak(string Code, ref int Index) {
        bool LineBreak = false;
        for (; Index < Code.Length; Index++) {
            if (Code[Index] is '\n' or '\r') {
                LineBreak = true;
            }
            else if (!char.IsWhiteSpace(Code[Index])) {
                break;
            }
        }
        return LineBreak;
    }
    private static Expression? EatExpression(string Code, ref int Index) {
        // Whitespace
        EatSpaces(Code, ref Index);

        // Bracket
        if (EatBracket(Code, ref Index) is Expression BracketExpression) {
            return BracketExpression;
        }
        // Quote String
        if (EatString(Code, ref Index, '\'') is string QuoteString) {
            return new StringExpression(QuoteString, false);
        }
        // Speech String
        if (EatString(Code, ref Index, '"') is string SpeechString) {
            return new StringExpression(SpeechString, true);
        }
        // Number
        if (EatNumber(Code, ref Index) is string Number) {
            // Decimal
            if (Number.Contains('.')) {
                return new DecimalExpression(Number);
            }
            // Integer
            else {
                return new IntegerExpression(Number);
            }
        }
        // Identifier
        if (EatIdentifier(Code, ref Index) is string Identifier) {
            // Method call (no arguments or brackets)
            return new CallExpression(new SelfExpression(), Identifier, []);
        }
        // Invalid
        return null;
    }
    private static string? EatIdentifier(string Code, ref int Index) {
        // Ensure start of identifier
        if (!Code[Index].IsValidIdentifier()) {
            return null;
        }

        // Build identifier while valid
        StringBuilder Builder = new();
        for (; Index < Code.Length; Index++) {
            if (Code[Index].IsValidIdentifier()) {
                Builder.Append(Code[Index]);
            }
            else {
                break;
            }
        }
        return Builder.ToString();
    }
    private static Expression? EatBracket(string Code, ref int Index) {
        // Ensure start of bracket expression
        if (Code[Index] is not '(') {
            return null;
        }
        Index++;

        // Eat expression inside brackets
        Expression? Expression = EatExpression(Code, ref Index)
            ?? throw new Exception("Expected expression inside bracket");
        Index++;

        // Ensure end of bracket expression
        EatSpaces(Code, ref Index);
        if (Code[Index] is not ')') {
            throw new Exception("Expected close bracket");
        }

        return Expression;
    }
    private static string? EatString(string Code, ref int Index, char StringSymbol) {
        // Ensure start of string
        if (Code[Index] != StringSymbol) {
            return null;
        }
        Index++;

        // Build until non-escaped end quote
        StringBuilder Builder = new();
        for (; Index < Code.Length; Index++) {
            if (Code[Index] == StringSymbol && Code[Index - 1] is not '\\') {
                break;
            }
            else {
                Builder.Append(Code[Index]);
            }
        }
        return Builder.ToString();
    }
    private static string? EatNumber(string Code, ref int Index) {
        // Ensure start of number
        if (!char.IsAsciiDigit(Code[Index])) {
            return null;
        }

        // Build number while digits or dot
        bool DecimalPart = false;
        StringBuilder Builder = new();
        for (; Index < Code.Length; Index++) {
            // Decimal place
            if (Code[Index] is '.') {
                // Start decimal part of number
                if (!DecimalPart) {
                    DecimalPart = true;
                    Builder.Append('.');
                }
                // Dot is not part of number
                else {
                    break;
                }
            }
            // Continue number
            else if (char.IsAsciiDigit(Code[Index]) || Code[Index] is '_') {
                Builder.Append(Code[Index]);
            }
            // End of number
            else {
                break;
            }
        }
        return Builder.ToString();
    }
}