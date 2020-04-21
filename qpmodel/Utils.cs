﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2020 Futurewei Corp.
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

using qpmodel.expr;

namespace qpmodel.utils
{
    class ObjectID
    {
        static int id_ = 0;

        internal static void Reset() { id_ = 0; }
        internal static int NewId() { return ++id_; }
        internal static int CurId() { return id_; }
    }

    public static class Utils
    {
        internal static string Tabs(int depth) => new string(' ', depth * 2);

        // this is shortcut for unhandled conditions - they shall be translated to 
        // related exceptional handling code later
        //
        public static void Checks(bool cond) => Debug.Assert(cond);
        public static void Assumes(bool cond) => Debug.Assert(cond);
        public static void Checks(bool cond, string message) => Debug.Assert(cond, message);
        public static void Assumes(bool cond, string message) => Debug.Assert(cond, message);

        public static string ToLower(this bool b) => b.ToString().ToLower();

        // a contains b?
        public static bool ContainsList<T>(this List<T> a, List<T> b) => !b.Except(a).Any();
        public static bool ListAEqualsB<T>(this List<T> a, List<T> b) => a.ContainsList(b) && b.ContainsList(a);

        // order insensitive
        //   if you need the list to be order sensitive compared, do it in Equals()
        public static int ListHashCode<T>(this List<T> l)
        {
            int hash = 0;
            if (l != null)
                l.ForEach(x => hash ^= x.GetHashCode());
            return hash;
        }

        public static string RetrieveQuotedString(this string str)
        {
            Debug.Assert(str.Count(x => x == '\'') == 2);
            var quotedstr = str.Substring(str.IndexOf('\''),
                                        str.LastIndexOf('\'') - str.IndexOf('\'') + 1);
            return quotedstr;
        }

        public static string RemoveStringQuotes(this string str)
        {
            Debug.Assert(str[0] == '\'' && str[str.Length - 1] == '\'');
            var dequote = str.Substring(1, str.Length - 2);
            return dequote;
        }

        public static bool StringLike(this string s, string pattern) {
            var regpattern = pattern.Replace("%", ".*");
            return Regex.IsMatch(s, regpattern);
        }

        // a[0]+b[1] => a+b 
        public static string RemovePositions(this string r)
        {
            do
            {
                int start = r.IndexOf('[');
                if (start == -1)
                    break;
                int end = r.IndexOf(']');
                Debug.Assert(end != -1);
                var middle = r.Substring(start + 1, end - start - 1);
                Debug.Assert(int.TryParse(middle, out int result));
                r = r.Replace($"[{middle}]", "");
            } while (r.Length > 0);
            return r;
        }

        public static void ReadCsvLine(string filepath, Action<string[]> action)
        {
            using var parser = new TextFieldParser(filepath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("|");
            while (!parser.EndOfData)
            {
                // Processing row
                string[] fields = parser.ReadFields();
                if (fields[fields.Length - 1].Equals(""))
                    Array.Resize(ref fields, fields.Length - 1);
                action(fields);
            }
        }
    }
}