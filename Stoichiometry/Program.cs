using Newtonsoft.Json.Linq;
using StoichiometryLibrary;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Stoichiometry
{
    /// <summary>
    /// CLI to calculate the mass of molecules
    /// </summary>
    /// Coder(s): Abdulaziz Hassan and Rayan Elkahlout
    internal class Program
    {
        static void Main(string[] args)
        {
            // when no arguments display help screen
            if (args.Length == 0)
            {
                DisplayHelpScreen();
            }
            else
            {
                switch (args[0])
                {
                    case "/?":
                        DisplayHelpScreen();
                        break;

                    case "/t":
                        DisplayPeriodcTable();
                        break;

                    default:
                        if (Regex.Match(args[0], "^/f:").Success)
                        {
                            string filepath = args[0].Remove(0, 3);

                            DisplayMassInFile(filepath);
                        }
                        else
                        {
                            DisplayMass(args);
                        }
                        break;
                }
            }
        }// end Main

        /// <summary>
        /// print app name, verions and authors
        /// </summary>
        static void DisplayVersion()
        {
            Console.WriteLine("\nStoichiometry 1.0.0 - 2024-02-05 - Abdulaziz Hassan, Rayan Elkahlout");
        }

        /// <summary>
        /// Help screen
        /// </summary>
        static void DisplayHelpScreen()
        {
            DisplayVersion();

            Console.WriteLine("\n Stoichiometry [/?] [formulas] [/t] [/f:filepath]\n");

            Console.WriteLine("{0, -12} {1}", " formulas", "specifies one or more white-space delimited molecular formulae for which to computer molecular mass.");
            Console.WriteLine("{0, -12} {1}", " /?", "Displays usage information.");
            Console.WriteLine("{0, -12} {1}", " /t", "Lists the elements in the periodic table.");
            Console.WriteLine("{0, -12} {1}", " /f:filepath", "Computes the molecular mass for each formula in the file 'filepath");
            Console.WriteLine("{0, -12} {1}", " filepath", "Specifies a text file containing molecular formulas, one per line.");
        } //end DisplayHelpScreen()


        /// <summary>
        /// Display the periodc table
        /// </summary>
        static void DisplayPeriodcTable()
        {
            DisplayVersion();

            try
            {
                IElement[] elements = PeriodicTable.Elements;

                Console.WriteLine();
                string format = "{0,-10} {1,-10} {2,-20} {3,20} {4,10} {5,10}";
                Console.WriteLine(format, "Atomic #", "Symbol", "Name", "Mass", "Period", "Group");
                Console.WriteLine(format, "--------", "------", "----", "----", "------", "-----");

                foreach (IElement element in elements)
                {
                    Console.WriteLine(format, element.AtomicNumber, element.Symbol, element.Name, element.AtomicMass, element.Period, element.Group);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }// end DisplayPeriodcTable()

        /// <summary>
        /// display mass of molecules in list
        /// </summary>
        /// <param name="molecules">list of molecules</param>
        static void DisplayMass(string[] molecules)
        {
            DisplayVersion();

            foreach(string molecule in molecules)
            {
                Molecule mol = new Molecule(molecule);
                if(mol.Valid)
                {
                    Console.WriteLine($"\n{mol.Formula} has a mass of {mol.CalcMass()}");
                    Console.WriteLine();

                    IMolecularElement[] composition = mol.GetComposition();

                    string format = "{0, 10} {1,-40} {2} x {3} = {4}";

                    foreach (IMolecularElement element in composition)
                    {
                        Console.WriteLine(format, element.Symbol, element.Name, element.AtomicMass, element.Multiplier, (element.AtomicMass * element.Multiplier));
                    }
                }
                else
                {
                    Console.WriteLine($"\n{mol.Formula} is NOT valid");
                }
            }
        }// end DisplayMass

        /// <summary>
        /// Display mass of molecules in a file
        /// each line is 1 molecule
        /// </summary>
        /// <param name="filepath">file path</param>
        static void DisplayMassInFile(string filepath)
        {
            try
            {
                string[] filecontents = File.ReadAllLines(filepath);
                DisplayMass(filecontents);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }// end DisplayMassInFile
    }
}
