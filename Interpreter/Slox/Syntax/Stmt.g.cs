﻿// WARNING! this code was generated by a tool

using Slox.Scanning;

#nullable enable

namespace Slox.Syntax;

public abstract record Stmt
{
    public abstract T Accept<T>(IVisitor<T> visitor);

    public interface IVisitor<T>
    {
        T VisitBlockStmt(Block stmt);
        T VisitExpressionStmt(Expression stmt);
        T VisitFunctionStmt(Function stmt);
        T VisitIfStmt(If stmt);
        T VisitVarStmt(Var stmt);
        T VisitPrintStmt(Print stmt);
        T VisitWhileStmt(While stmt);
    }

    public record Block(List<Stmt> Statements) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBlockStmt(this);
    }

    public record Expression(Expr Expr) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitExpressionStmt(this);
    }

    public record Function(Token Name, List<Token> Params, List<Stmt> Body) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitFunctionStmt(this);
    }

    public record If(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitIfStmt(this);
    }

    public record Var(Token Name, Expr? Initializer) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitVarStmt(this);
    }

    public record Print(Expr Expr) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrintStmt(this);
    }

    public record While(Expr Condition, Stmt Body) : Stmt()
    {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitWhileStmt(this);
    }

}
