using System;


namespace FakturowniaService.task
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FakturTaskAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JobStatusTaskAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HostCheckTaskAttribute : Attribute { }

}
