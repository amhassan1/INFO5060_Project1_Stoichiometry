using System.Text.RegularExpressions;

namespace StoichiometryLibrary
{
    /// <summary>
    /// Class to represent a chemical molecule
    /// </summary>
    public class Molecule
    {
        /// <summary>
        /// Default constructor, sets Formula to an empty string
        /// </summary>
        public Molecule()
        {
            Formula = "";
        }

        /// <summary>
        /// Create molecule object with formula passed
        /// </summary>
        /// <param name="formula"></param>
        public Molecule(string formula)
        {
            Formula = formula;
        }

        /// <summary>
        /// Element composition of molecule
        /// </summary>
        private List<Element> _composition = new List<Element>();

        /// <summary>
        /// Formula for molecule
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// if molecule is vaild
        /// </summary>
        public bool Valid => IsValid(Formula);

        /// <summary>
        /// Calculate mass of molecule
        /// </summary>
        /// <returns>mass of molecule</returns>
        public double CalcMass()
        {
            if(Valid)
            {
                double mass = 0;
                IMolecularElement[] molecules = GetComposition();
                foreach(IMolecularElement molecule in molecules)
                {
                    mass += (molecule.AtomicMass * molecule.Multiplier);
                }

                return mass;
            }

            return 0;
        }

        /// <summary>
        /// Get the compsiotion of the molecule (i.e. elements and their count in the molecule)
        /// </summary>
        /// <returns>Array of Molecular Elements in molecule</returns>
        public IMolecularElement[] GetComposition()
        {
            List<IMolecularElement> molecularElements = new();
            if (Valid)
            {
                _composition.Clear();
                GetCompositionHelper(Formula, 1, 0);
                return _composition.ToArray();
            }

            return molecularElements.ToArray();
        }

        /// <summary>
        /// Gets and parses the subscript of a sub-formula and removes subscript from the sub-formula string
        /// </summary>
        /// <param name="sub_formula">refrence to sub-formula (with brackets and subscript)</param>
        /// <param name="closeBracket">index of closing bracket</param>
        /// <returns>Parsed subscript or 1 if there isn't subscript</returns>
        private ushort GetNumberAfterBracket(ref string sub_formula, int closeBracket)
        {
            string strNumAfterBracket = "";
            int i = closeBracket + 1;
            while (i < sub_formula.Length)
            {
                if (!char.IsDigit(sub_formula[i]))
                {
                    break;
                }
                strNumAfterBracket += sub_formula[i];
                sub_formula = sub_formula.Remove(i, 1);
            }
            ushort numAfterBracket;
            bool conveted = ushort.TryParse(strNumAfterBracket, out numAfterBracket);
            if (!conveted)
            {
                numAfterBracket = 1;
            }

            return numAfterBracket;
        }

        /// <summary>
        /// recursive function that populates the molecule element composition list 
        /// </summary>
        /// <param name="sub_formula">Chemical Formula/Sub-formula (without outside brackets and subscript)</param>
        /// <param name="subscript">Subscript of formula</param>
        /// <param name="depth">depth of the recursive function</param>
        private void GetCompositionHelper(string sub_formula, ushort subscript, int depth)
        {
            // parenthesis indexes
            int openBracket = sub_formula.IndexOf('(');
            int closeBracket = sub_formula.LastIndexOf(')');
            int lastOpenBracket = sub_formula.LastIndexOf('(');
            int firstCloseBracket = sub_formula.IndexOf(')');

            // speical case when there is two sub_formulas (not nested)
            if (depth == 0 && lastOpenBracket > firstCloseBracket)
            {
                ushort numAfterBracket = GetNumberAfterBracket(ref sub_formula, firstCloseBracket);
                GetCompositionHelper(sub_formula.Substring(openBracket + 1, firstCloseBracket - openBracket - 1), numAfterBracket, depth++);
                sub_formula = sub_formula.Remove(openBracket, firstCloseBracket - openBracket + 1);
            }

            // get parenthesis indexes agian in-case removed some
            openBracket = sub_formula.IndexOf('(');
            closeBracket = sub_formula.LastIndexOf(')');

            // No brackets (smallest sub-formula)
            if (openBracket != -1 && closeBracket != -1)
            {
                ushort numAfterBracket = GetNumberAfterBracket(ref sub_formula, closeBracket);

                GetCompositionHelper(sub_formula.Substring(openBracket + 1, closeBracket - openBracket - 1), numAfterBracket, depth++);
                sub_formula = sub_formula.Remove(openBracket, closeBracket - openBracket+1);
            }

            // process each single element in sub-formula
            Regex regex = new Regex("[A-Z][a-z]*[0-9]*");

            foreach (Match match in regex.Matches(sub_formula))
            {
                string strNumberOfAtoms = new string(match.Value.Where(Char.IsDigit).ToArray());
                ushort numberOfAtoms = 1;
                if(strNumberOfAtoms.Length > 0)
                {
                    numberOfAtoms = ushort.Parse(strNumberOfAtoms);
                }

                string symbol = new string(match.Value.Where(Char.IsAsciiLetter).ToArray());

                bool elementFound = false;

                // if element already in composition increment multiplier
                foreach (Element element in _composition)
                {
                    if(element.Symbol == symbol)
                    {
                        elementFound = true;
                        ushort numberOfOccurrences = (ushort)(numberOfAtoms * subscript);
                        element.Multiplier += numberOfOccurrences;
                    }
                }
                // add element to composition
                if(!elementFound)
                {
                    Element element = new Element() { Symbol = "", Name = "" };

                    // find element in periodic table
                    IElement elementFromPeroidcTable = PeriodicTable.Elements.First(e => e.Symbol == symbol);

                    element.Name = elementFromPeroidcTable.Name;
                    element.Symbol = elementFromPeroidcTable.Symbol;
                    element.Period = elementFromPeroidcTable.Period;
                    element.Group = elementFromPeroidcTable.Group;
                    element.AtomicMass = elementFromPeroidcTable.AtomicMass;
                    element.AtomicNumber = elementFromPeroidcTable.AtomicNumber;

                    ushort numberOfOccurrences = (ushort)(numberOfAtoms * subscript);
                    element.Multiplier += numberOfOccurrences;

                    _composition.Add(element);
                }
            }
        }//End method

        /// <summary>
        /// Validate formula
        /// </summary>
        /// <param name="formula">formula to be validated</param>
        /// <returns>true if valid, otherwise false</returns>
        private bool IsValid(string formula)
        {
            // 1. not empty
            if (string.IsNullOrEmpty(formula))
            {
                return false;
            }

            // 2. no spaces
            if(formula.Contains(' '))
            {
                return false;
            }

            string newFormula = "";

            // 3. check valid structure

            // brackets added to work with regex if formula doesn't start with bracket
            if (formula.StartsWith('(') && formula.EndsWith(')'))
            {
                newFormula = formula;
            }
            else
            {
                newFormula = "(" + formula + ")";
            }

            // if first letter or number is not uppercase letter
            char firstLetter = newFormula.First(Char.IsLetterOrDigit);
            if (!Char.IsUpper(firstLetter))
            {
                return false;
            }

            Regex subformulaCapture = new Regex("\\([^()]+\\)\\d*");

            // remove valid subformulas
            while(subformulaCapture.IsMatch(newFormula))
            {
                string matchString = subformulaCapture.Match(newFormula).Value;
                int indexOfSubstring = newFormula.IndexOf(matchString);
                newFormula = (indexOfSubstring < 0) ? newFormula : newFormula.Remove(indexOfSubstring, matchString.Length);
            }

            // if all valid subformulas removed and characters left in string
            if(!string.IsNullOrEmpty(newFormula))
            {
                return false;
            }

            // 4. check valid symbols
            Regex elementCapture = new Regex("[A-Z][a-z]*");

            bool validSymbol = false;

            // all possible chemical symbols
            MatchCollection elementMatches = elementCapture.Matches(formula);

            if (elementMatches.Count <= 0)
            {
                return false;
            }

            // verify symbol is in periodic table
            foreach (Match match in elementMatches)
            {
                foreach (Element element in PeriodicTable.Elements)
                {
                    if (match.Value == element.Symbol)
                    {
                        validSymbol = true;
                        break;
                    }
                }
                if (!validSymbol)
                {
                    return false; // found a non valid symbol return early
                }
                validSymbol = false; // reset flag for next element
            }

            return true;
        }// end IsValid
    }
}
