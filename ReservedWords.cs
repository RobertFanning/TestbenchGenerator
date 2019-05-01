using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VHDLparser {
	// Static class containing a list of reserved VHDL words.
	public static class ReservedWords {

		static string[] words = {
			"abs",
			"access",
			"after",
			"alias",
			"all",
			"and",
			"architecture",
			"array",
			"assert",
			"attribute",
			"begin",
			"block",
			"body",
			"buffer",
			"bus",
			"case",
			"component",
			"configuration",
			"constant",
			"disconnect",
			"downto",
			"else",
			"elsif",
			"end",
			"entity",
			"exit",
			"file",
			"for",
			"function",
			"generate",
			"generic",
			"group",
			"guarded",
			"if",
			"impure",
			"in",
			"inertial",
			"inout",
			"is",
			"label",
			"library",
			"linkage",
			"literal",
			"loop",
			"map",
			"mod",
			"nand",
			"new",
			"next",
			"nor",
			"not",
			"null",
			"of",
			"on",
			"open",
			"or",
			"others",
			"out",
			"package",
			"port",
			"postponed",
			"procedure",
			"process",
			"protected",
			"pure",
			"range",
			"record",
			"register",
			"reject",
			"rem",
			"report",
			"return",
			"rol",
			"ror",
			"select",
			"severity",
			"shared",
			"signal",
			"sla",
			"sll",
			"sra",
			"srl",
			"subtype",
			"then",
			"to",
			"transport",
			"type",
			"unaffected",
			"units",
			"until",
			"use",
			"variable",
			"wait",
			"when",
			"while",
			"with",
			"xnor",
			"xor"
		};

		public static bool IsReservedWord (string compare) {
			return words.Contains (compare);
		}
	}
}