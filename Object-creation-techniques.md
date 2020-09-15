# object-creation-techniques
A collection of object creation techniques

## Linq expressions
From:
https://rogerjohansson.blog/2008/02/28/linq-expressions-creating-objects/

First we need a delegate declaration:

```c#
delegate T ObjectActivator<T>(params object[] args);
```

We will use this delegate in order to create our instances.
We also need a generator that can compile our delegates from expression trees:

```c#
public static ObjectActivator<T> GetActivator<T>
    (ConstructorInfo ctor)
{
    Type type = ctor.DeclaringType;
    ParameterInfo[] paramsInfo = ctor.GetParameters();                  

    //create a single param of type object[]
    ParameterExpression param =
        Expression.Parameter(typeof(object[]), "args");
 
    Expression[] argsExp =
        new Expression[paramsInfo.Length];            

    //pick each arg from the params array 
    //and create a typed expression of them
    for (int i = 0; i < paramsInfo.Length; i++)
    {
        Expression index = Expression.Constant(i);
        Type paramType = paramsInfo[i].ParameterType;              

        Expression paramAccessorExp =
            Expression.ArrayIndex(param, index);              

        Expression paramCastExp =
            Expression.Convert (paramAccessorExp, paramType);              

        argsExp[i] = paramCastExp;
    }                  

    //make a NewExpression that calls the
    //ctor with the args we just created
    NewExpression newExp = Expression.New(ctor,argsExp);                  

    //create a lambda with the New
    //Expression as body and our param object[] as arg
    LambdaExpression lambda =
        Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);              

    //compile it
    ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
    return compiled;
}
```

It’s a bit bloated but keep in mind that this version is completely dynamic and you can pass any constructor with any arguments to it.

Once we have the generator and the delegate in place, we can start creating instances with it:

```c#
ConstructorInfo ctor = typeof(Created).GetConstructors().First();
ObjectActivator<Created> createdActivator = GetActivator<Created>(ctor);
...
//create an instance:
Created instance = createdActivator (123, "Roger");
```

 And that’s it.
