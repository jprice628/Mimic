using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mimic.Utils
{
    // Provides a simple method for constructing a pipeline from a list of functions.
    //
    // Notes
    // - Steps are executed in the order that they are defined.
    // - Steps take the basic form: Task StepName(TContext ctx, Func<TContext, Task> next);
    //
    // Basic usage:
    //    Main()
    //    {
    //        var context = new object();
    //        var pipeline = new Pipeline<object>()
    //            .Use(Operation1)
    //            .Use(Operation2)
    //            .Use(Operation3)
    //            .Build();
    //        pipeline.Invoke(context).Wait();
    //    }
    //
    //    Task Operation1(object context, Func<Task> next)
    //    {
    //        // Do something.
    //        next();
    //    }

    /// <summary>
    /// Provides a simple mechanism for constructing a pipeline from a list 
    /// of functions. This is primarily used to setup the parse pipeline used 
    /// for defining new virtual services. See ServiceFunctions.InitParseServiceDescPipeline
    /// </summary>
    public class Pipeline<TContext>
    {
        private readonly List<Func<TContext, Func<Task>, Task>> steps;

        public Pipeline()
        {
            steps = new List<Func<TContext, Func<Task>, Task>>();
        }

        /// <summary>
        /// Adds a function to the pipeline. Functions are invoked in the 
        /// order in which they are added.
        /// </summary>
        public Pipeline<TContext> Use(Func<TContext, Func<Task>, Task> step)
        {
            RequiresArgument.NotNull(step, "step");

            steps.Add(step);
            return this;
        }

        /// <summary>
        /// Takes the list of functions added via the Use method and 
        /// constructs a function that invokes them in order.
        /// </summary>
        public Func<TContext, Task> Build()
        {
            if (steps.Count == 0)
            {
                throw new InvalidOperationException("No steps have been added to the pipeline.");
            }

            Func<TContext, Task> invoke = (TContext context) => { return Invoke(context, 0); };
            return invoke;
        }

        private Task Invoke(TContext context, int index)
        {
            if (index >= steps.Count)
            {
                return Task.CompletedTask;
            }
            else
            {
                return steps[index](context, () => Invoke(context, index + 1));
            }
        }
    }
}
