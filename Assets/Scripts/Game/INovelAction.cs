using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface INovelAction
{
    /// <summary>
    /// Runs the action if it passes the debug stage
    /// </summary>
    /// <param name="line">Full line to the action to run</param>
    void Run(string[] line);
    /// <summary>
    /// Debugs the action and returns appropriate feedback
    /// </summary>
    /// <param name="line">Full line to debug</param>
    NovelDebugInfo Working(string[] line);
}