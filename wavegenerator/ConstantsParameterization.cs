using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace wavegenerator
{
    public class ConstantsParameterization
    {
        public static void ParameterizeConstants()
        {
            Console.WriteLine("You can now configure the parameters which will influence the track generation.");
            Console.WriteLine("Note that if you use randomization, the program will likely never be able to exactly reproduce a track again - even with the same parameters, so make sure you keep it in case it's good!");
            Console.WriteLine();

            var consoleWriter = new ColorConsole.ConsoleWriter();
            foreach (var field in typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                bool isValid;
                do
                {
                    isValid = true;
                    var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                    consoleWriter.Write($"{field.Name}: ", ConsoleColor.Yellow);
                    consoleWriter.WriteLine(Regex.Replace(descriptionAttribute?.Description ?? string.Empty, @"\n\s*", "\n"));

                    consoleWriter.Write($"{field.Name}? ({field.GetValue(null)}) : ", ConsoleColor.White);
                    var newValString = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newValString))
                    {
                        try
                        {
                            var newVal = Parse(newValString, field.FieldType);
                            Validate(field, newVal);
                            field.SetValue(null, newVal);
                        }
                        catch(TargetInvocationException e)
                        {
                            consoleWriter.WriteLine(e.InnerException.Message, ConsoleColor.Red);
                            isValid = false;
                        }
                        catch (Exception e)
                        {
                            consoleWriter.WriteLine(e.Message, ConsoleColor.Red);
                            isValid = false;
                        }
                    }
                    consoleWriter.WriteLine($"{field.Name} = {field.GetValue(null)}", ConsoleColor.Gray);
                    Console.WriteLine();
                } while (!isValid);

            }
        }

        public static void Validate(FieldInfo field, object newVal)
        {
            if (
                (newVal is double d && d < 0) ||
                (newVal is int i && i < 0)
                ) //nothing's allowed to be negative unless specifically marked
            {
                throw new InvalidOperationException($"{field.Name} must be >= 0");
            }

            //has it got a specific validation method
            var validationMethod = typeof(Constants).GetMethod($"{field.Name}Validation", BindingFlags.Public | BindingFlags.Static);
            if (validationMethod != null)
            {
                validationMethod.Invoke(null, new[] { newVal });
            }
        }

        private static readonly Dictionary<char, bool> boolLookups = new Dictionary<char, bool>
        {
            {'y', true },
            {'n', false }
        };

        public static object Parse(string stringval, Type type)
        {
            if (type == typeof(bool) && boolLookups.TryGetValue(stringval.Trim().ToLower().FirstOrDefault(), out bool b)) return b;

            var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (parseMethod != null)
            {
                return parseMethod.Invoke(null, new[] { stringval });
            }
            else
            {
                return Convert.ChangeType(stringval, type);
            }
        }
    }
}
