using System;

namespace Limbo.Umbraco.Spa.Models.Flow;

/// <summary>
/// Class representing an action group in the SPA page life cycle.
/// </summary>
public class SpaActionGroup {

    /// <summary>
    /// Gets the predicate determining whether the action group should be executed.
    /// </summary>
    public Predicate<SpaRequest> Run { get; }

    /// <summary>
    /// Gets an array with the actions of the group.
    /// </summary>
    public Action<SpaRequest>[] Actions { get; }

    /// <summary>
    /// Initializes a new action group from the specified array of <paramref name="actions"/>.
    /// </summary>
    /// <param name="actions">The actions that should make up the action group.</param>
    public SpaActionGroup(params Action<SpaRequest>[] actions) {
        Run = _ => true;
        Actions = actions;
    }

    /// <summary>
    /// Initializes a new action group from the specified array of <paramref name="actions"/>.
    /// </summary>
    /// <param name="run">A predicate returning whether the action group should be executed.</param>
    /// <param name="actions">The actions that should make up the action group.</param>
    public SpaActionGroup(Predicate<SpaRequest> run, params Action<SpaRequest>[] actions) {
        Run = run;
        Actions = actions;
    }

}