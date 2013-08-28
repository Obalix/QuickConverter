﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QuickConverter.Tokens
{
	public class ArgumentListToken : TokenBase
	{
		private char open;
		private char close;
		private Type assignmentType;
		private bool allowSubLists;
		internal ArgumentListToken(char open, char close, Type assignmentType = null)
		{
			this.open = open;
			this.close = close;
			this.assignmentType = assignmentType;
			allowSubLists = false;
		}

		internal ArgumentListToken(char open, char close, bool allowSubLists)
		{
			this.open = open;
			this.close = close;
			assignmentType = null;
			this.allowSubLists = allowSubLists;
		}

		internal List<TokenBase> Arguments { get; private set; }

		internal override bool TryGetToken(ref string text, out TokenBase token)
		{
			token = null;
			var list = new List<TokenBase>();
			List<string> split;
			string temp = text;
			if (!TrySplitByCommas(ref temp, open, close, out split))
				return false;
			foreach (string str in split)
			{
				TokenBase newToken;
				string s = str.Trim();
				if (allowSubLists && s.StartsWith(open.ToString()) && s.EndsWith(close.ToString()))
				{
					if (new ArgumentListToken(open, close).TryGetToken(ref s, out newToken))
						list.Add(newToken);
					else
						return false;
				}
				else if (assignmentType != null)
				{
					if (new AssignmentToken(assignmentType).TryGetToken(ref s, out newToken))
						list.Add(newToken);
					else
						return false;
				}
				else
				{
					if (EquationTokenizer.TryEvaluateExpression(str.Trim(), out newToken))
						list.Add(newToken);
					else
						return false;
				}
			}
			token = new ArgumentListToken('\0', '\0') { Arguments = list };
			text = temp;
			return true;
		}

		public override Expression GetExpression(List<ParameterExpression> parameters, Type dynamicContext = null)
		{
			throw new NotImplementedException();
		}
	}
}