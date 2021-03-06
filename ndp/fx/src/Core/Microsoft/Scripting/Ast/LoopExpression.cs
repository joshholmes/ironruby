/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System.Diagnostics;
using System.Dynamic.Utils;

#if SILVERLIGHT
using System.Core;
#endif

namespace System.Linq.Expressions {
    /// <summary>
    /// Represents an infinite loop. It can be exited with "break".
    /// </summary>
#if !SILVERLIGHT
    [DebuggerTypeProxy(typeof(Expression.LoopExpressionProxy))]
#endif
    public sealed class LoopExpression : Expression {
        private readonly Expression _body;
        private readonly LabelTarget _break;
        private readonly LabelTarget _continue;

        internal LoopExpression(Expression body, LabelTarget @break, LabelTarget @continue) {
            _body = body;
            _break = @break;
            _continue = @continue;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type {
            get { return _break == null ? typeof(void) : _break.Type; }
        }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public sealed override ExpressionType NodeType {
            get { return ExpressionType.Loop; }
        }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is the body of the loop.
        /// </summary>
        public Expression Body {
            get { return _body; }
        }

        /// <summary>
        /// Gets the <see cref="LabelTarget"/> that is used by the loop body as a break statement target.
        /// </summary>
        public LabelTarget BreakLabel {
            get { return _break; }
        }

        /// <summary>
        /// Gets the <see cref="LabelTarget"/> that is used by the loop body as a continue statement target.
        /// </summary>
        public LabelTarget ContinueLabel {
            get { return _continue; }
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor) {
            return visitor.VisitLoop(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="ContinueLabel" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public LoopExpression Update(LabelTarget breakLabel, LabelTarget continueLabel, Expression body) {
            if (breakLabel == BreakLabel && continueLabel == ContinueLabel && body == Body) {
                return this;
            }
            return Expression.Loop(body, breakLabel, continueLabel);
        }
    }

    public partial class Expression {
        /// <summary>
        /// Creates a <see cref="LoopExpression"/> with the given body.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="LoopExpression"/>.</returns>
        public static LoopExpression Loop(Expression body) {
            return Loop(body, null);
        }

        /// <summary>
        /// Creates a <see cref="LoopExpression"/> with the given body and break target.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="LoopExpression"/>.</returns>
        public static LoopExpression Loop(Expression body, LabelTarget @break) {
            return Loop(body, @break, null);
        }

        /// <summary>
        /// Creates a <see cref="LoopExpression"/> with the given body.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="LoopExpression"/>.</returns>
        public static LoopExpression Loop(Expression body, LabelTarget @break, LabelTarget @continue) {
            RequiresCanRead(body, "body");
            ContractUtils.Requires(@continue == null || @continue.Type == typeof(void), "continue", Strings.LabelTypeMustBeVoid);
            return new LoopExpression(body, @break, @continue);
        }
    }
}
