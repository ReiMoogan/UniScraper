using HotChocolate.Types;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Models.V2.GraphQL;

public class ClassType : ObjectType<Class>
{
    protected override void Configure(IObjectTypeDescriptor<Class> descriptor)
    {
        descriptor.Field(o => o.Id).Type<IdType>();
        descriptor.Field(o => o.Term).Type<IntType>();
        descriptor.Field(o => o.CourseReferenceNumber).Type<IntType>();
        descriptor.Field(o => o.CourseNumber).Type<StringType>();
        descriptor.Field(o => o.CampusDescription).Type<StringType>();
        descriptor.Field(o => o.CourseTitle).Type<StringType>();
        descriptor.Field(o => o.CreditHours).Type<IntType>();
        descriptor.Field(o => o.MaximumEnrollment).Type<IntType>();
        descriptor.Field(o => o.Enrollment).Type<IntType>();
        descriptor.Field(o => o.SeatsAvailable).Type<IntType>();
        descriptor.Field(o => o.WaitCapacity).Type<IntType>();
        descriptor.Field(o => o.WaitAvailable).Type<IntType>();
    }
}