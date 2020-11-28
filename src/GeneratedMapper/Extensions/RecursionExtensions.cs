using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Extensions
{
    public static class RecursionExtensions
    {
        public static IEnumerable<TReturn> DoRecursionSafe<TSubject, TReturn>(
            this TSubject startingSubject, 
            Func<TSubject, IEnumerable<TReturn>> action, 
            Func<TSubject, IEnumerable<TSubject?>?> nextSubjects)
        {
            var processedSubjects = new HashSet<TSubject>();

            var subjectList = new List<TSubject> { startingSubject };

            do
            {
                var subjectsToProcessThisCycle = subjectList.ToList();

                subjectList.Clear();

                foreach (var subject in subjectsToProcessThisCycle)
                {
                    if (processedSubjects.Contains(subject))
                    {
                        continue;
                    }

                    processedSubjects.Add(subject);

                    foreach (var element in action.Invoke(subject))
                    {
                        yield return element;
                    }
                    
                    var nextSubjectsToAdd = nextSubjects.Invoke(subject);
                    if (nextSubjectsToAdd != null)
                    {
                        subjectList.AddRange(nextSubjectsToAdd.Where(x => x is not null).Cast<TSubject>());
                    }
                }

            } while (subjectList.Count > 0);
        }
    }
}
