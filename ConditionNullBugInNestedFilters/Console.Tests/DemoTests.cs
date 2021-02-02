using System;
using System.Collections.Generic;
using System.Linq;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Console.Tests
{
    public class DemoTests
    {

        [Fact]
        public void FilterOnConditionOperatorNull_DoesNotWorkFromVersion1_51_1()
        {
            //This test is working in earlier versions of FakeXrmEasy.9 (version 1.51.0), but not working in all later versions.

            var context = new XrmFakedContext();

            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = "test"
            };

            var employment = new Employment
            {
                Id = Guid.NewGuid(),
                PersonId = person.ToEntityReference(),
                StartDate = null,
                EndDate = null
            };

            context.Initialize(new List<Entity>
            {
                person,
                employment

            });

            var service = context.GetOrganizationService();

            var query = new QueryExpression
            {
                EntityName = Person.EntityLogicalName,
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = Person.EntityLogicalName,
                        LinkFromAttributeName = Person.AttributeLogicalNames.Id,
                        LinkToEntityName = Employment.EntityLogicalName,
                        LinkToAttributeName = Employment.AttributeLogicalNames.PersonId,
                        JoinOperator = JoinOperator.Inner,
                        EntityAlias = Employment.EntityLogicalName,
                        Columns = new ColumnSet(
                            Employment.AttributeLogicalNames.Id,
                            Employment.AttributeLogicalNames.StartDate
                        ),
                        LinkCriteria = new FilterExpression
                        {
                            Filters =
                            {
                                new FilterExpression(LogicalOperator.And)
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression(Employment.AttributeLogicalNames.EndDate, ConditionOperator.Null)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var results = service.RetrieveMultiple(query).Entities.Cast<Person>().ToList();
            Assert.Single(results);
        }
    }
}