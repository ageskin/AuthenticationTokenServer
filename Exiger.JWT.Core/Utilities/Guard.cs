using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Utilities
{
    public static class Guard
    {
        /// <summary>
        /// Checks if an object reference is null 
        /// </summary>
        /// <typeparam name="T">The underlying type of argument</typeparam>
        /// <param name="argument">The object reference to check</param>
        /// <param name="name">The name of the argument</param>
        /// <returns>argument if it's not null</returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if argument is null</exception>
        public static T AgainstNull<T>(T argument, string name = null)
        {
            if (argument == null)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    throw new ArgumentNullException(name);
                }
            }

            return argument;
        }

        public static string AgainstNullAndEmpty(string argument, string name = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    throw new ArgumentNullException(name);
                }
            }

            return argument;
        }

		public static IEnumerable<T> AgainstNullAndEmpty<T>(IEnumerable<T> argument, string name = null)
		{
			if (argument == null || argument.Count() == 0)
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					throw new ArgumentNullException();
				}
				else
				{
					throw new ArgumentNullException(name);
				}
			}

			return argument;
		}
    }
}
