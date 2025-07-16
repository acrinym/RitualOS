namespace RitualOS.Services
{
    /// <summary>
    /// Plugin interface for applying custom rewrite transformations.
    /// </summary>
    public interface IRewriteRule
    {
        /// <summary>
        /// Transform the given input string and return the result.
        /// </summary>
        string Apply(string input);
    }
}

