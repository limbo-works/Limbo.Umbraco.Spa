using System;

namespace Skybrud.Umbraco.Spa.Models.Flow {

    public class SpaActionGroup {

        /// <summary>
        /// Gets the predicate determining whether the action group should be executed.
        /// </summary>
        public Predicate<SpaRequest> Run { get; }

        /// <summary>
        /// Gets an array with the actions of the group.
        /// </summary>
        public Action<SpaRequest>[] Actions { get; }

        public SpaActionGroup(params Action<SpaRequest>[] actions) {
            Run = r => true;
            Actions = actions;
        }

        public SpaActionGroup(Predicate<SpaRequest> run, params Action<SpaRequest>[] actions) {
            Run = run;
            Actions = actions;
        }

    }

}