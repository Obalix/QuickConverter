﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace QuickConverter.Tokens
{
	public class InstanceMemberToken : TokenBase, IPostToken
	{
		internal InstanceMemberToken()
		{
		}

		private string memberName;
		TokenBase IPostToken.Target { get; set; }
		internal override bool TryGetToken(ref string text, out TokenBase token)
		{
			token = null;
			string temp = text;
			if (temp.Length < 2 || temp[0] != '.' || (!Char.IsLetter(temp[1]) && temp[1] != '_'))
				return false;
			int count = 2;
			while (count < temp.Length && (Char.IsLetterOrDigit(temp[count]) || temp[count] == '_'))
				++count;
			if (count < temp.Length && temp[count] == '(')
				return false;
			string name = temp.Substring(1, count - 1);
			text = temp.Substring(count);
			token = new InstanceMemberToken() { memberName = name };
			return true;
		}

		public override Expression GetExpression(List<ParameterExpression> parameters, Type dynamicContext = null)
		{
			CallSiteBinder binder = Binder.GetMember(CSharpBinderFlags.None, memberName, dynamicContext ?? typeof(object), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
			return Expression.Dynamic(binder, typeof(object), (this as IPostToken).Target.GetExpression(parameters, dynamicContext));
		}
	}
}