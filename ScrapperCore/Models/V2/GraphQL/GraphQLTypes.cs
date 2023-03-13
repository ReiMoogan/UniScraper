using HotChocolate.Types;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Models.V2.GraphQL;

public class GraphQLTypes : ObjectType<Class>
{
    protected override void Configure(IObjectTypeDescriptor<Class> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class DescriptionType : ObjectType<Description>
{
    protected override void Configure(IObjectTypeDescriptor<Description> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class FacultyType : ObjectType<Faculty>
{
    protected override void Configure(IObjectTypeDescriptor<Faculty> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class LinkedSectionType : ObjectType<LinkedSection>
{
    protected override void Configure(IObjectTypeDescriptor<LinkedSection> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class MeetingType : ObjectType<Meeting>
{
    protected override void Configure(IObjectTypeDescriptor<Meeting> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class MeetingTypeType : ObjectType<MeetingType>
{
    protected override void Configure(IObjectTypeDescriptor<MeetingType> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class ProfessorType : ObjectType<Professor>
{
    protected override void Configure(IObjectTypeDescriptor<Professor> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class StatType : ObjectType<Stat>
{
    protected override void Configure(IObjectTypeDescriptor<Stat> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}

public class SubjectType : ObjectType<Subject>
{
    protected override void Configure(IObjectTypeDescriptor<Subject> descriptor)
    {
        descriptor.BindFieldsImplicitly();
    }
}