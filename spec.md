# TERSE

## What is Terse?
**Terse** is a recreational programming language designed for code golfing. Code golfing is the act of writing a program in as few bytes as possible to solve a problem.

## How does Terse accomplish that?
* Functions are 1 byte long
* Each non-unary function has 4 "tiers", which represent how many symbols after the function are arguments. This means that brackets can be omitted in many instances.
* Shortcuts for 1-3 character strings
* It has it's own code page

This document is a basic rundown on how to program in Terse.

## Datatypes
Terse has 3 data types: Strings, Numbers, and Lists

### Strings
Strings literals are defined as `“string“` (note that this is not a regular double quote). You can leave out the closing double quote if the string is at the end of a program, saving a byte. Additionally, there are shortcuts for strings that are 1-3 characters long.

* 1 character strings can be defined as `'s`
* 2 character strings can be defines as `’st`
* 3 character strings can be defines as `‘str`

For larger strings, a string compression feature is coming.

Note that newline characters are not allowed in strings just yet. If you want to have a newline character in your string, use `￥` instead.

### Numbers
Numbers literals are what you expect: a string of digits, with an optional decimal point. Here are some examples:
```
.12
53
0.421
421659
```

For larger numbers, a compressed number literal form is being worked on. Anything between `"` is a compressed number literal, which is evaluated as base-255 (with the Terse code page).

### Lists
Lists are what they sound like: a list of values (which can be strings, numbers, or other lists).
Currently there is no list literal (being worked on), but you can define lists with the `当` function (which pairs it's left and right arguments into a length 2 list).

## Input
The first four inputs are saved to the input variable `哦`,`情`,`作`, and `跟`

The rest of the inputs are saved to the list `面`.

## Variables

There are 19 variables. Their values are listed here:

```
//Inputs
["哦"] = 0,
["情"] = 0,
["作"] = 0,
["跟"] = 0,
// Input Array
["面"] = 0,

//Unused variables (for now)
["诉"] = 0,
["爱"] = 0,

["已"] = " ",
["之"] = "\n",
["问"] = "",
["错"] = -1,
["孩"] = 10,
//Function parameters
["斯"] = 16,
["成"] = 15,
["它"] = 14,
["感"] = 13,
["干"] = 12,
["法"] = 11,

["电"] = 100,
//Assign
["间"] = 1000,
```
The function parameter variables have their values replaced when they are used inside a lambda. There will be more on this in the higher order functions section.

The assign variable is special because it's value is changed by the `经` unary function. When `经` is called, it returns it's caller. But what it also does is that it stores the value of the caller into the `间` variable for future reference.

## Functions
To call a function, simply place the function name right in front of the expression that you want to be the caller. For example, if you want to call the "double" function `地` on the first input variable `哦` (assuming that `哦` is a number):

```
哦地
```

And that's a program that doubles its input!

The behavior of a function changes based on it's caller types and argument types. For example, `地` doubles its caller if the caller is a number, but it also returns the first element if the caller is a list. Additionally, it returns the first character of a string if that is what the caller is.

The full list of functions can be found [here]()

There are 3 types of functions:

### Unary Functions
Unary functions don't take any arguments beside its caller. The function `地` that was just used is an example of an unary function. All of the below are unary functions:

```
经妈用打地再因呢女告最手前找行快而死先像等被从明中
```

### Binary Functions

Binary functions receive both a caller and an argument.

### Higher Order Functions

Higher order functions receive a caller and a lambda as it's second argument. The way the lambda works is that it is re-evaluated each time the higher order function uses it. The function can pass arguments into the lambda. 

Example: (`点` here is a higher order function, the rest are unary)
```
点最因中
```

`点` = map

`最` = string to code point

`因` = increment

`中` = code point to string

## Tiers and Autofills
### Autofills
If you do not give an argument or a caller to a function, Terse will automagically fill in the value for you. Here is how the value of an autofill is decided:

If the autofill is the parameter of a higher order function:
- Return the first input

If we are inside a higher-order function
 - The 'parameter variables' are 斯,成,它,感,干,法
 - They are placed upon a stack
 - Based on how many arguments are needed, that number is popped from the stack
 - E.g some higher-order functions require 3 parameters, some only need 2 or 1
 - If there are at least two parameter variables, the first one will be designated as the first autofill
 - and the second will be designated as the second autofill
 
Else if we are not inside a higher-order function
 - The first autofill will be the first input passed to the program
 - The second autofill will be the second input passed to the program
Now we have our first and second autofill
 - If there are zero autofill, just use 0
 - If there is only one autofill, use that one autofill
 - If there are at least two autofills defined:
   - If the expression is an argument to a function and doesn't call any other function, use the second autofill
   - Else just use the first

Remember our double the number program? We can shave off one byte off of it by taking out the reference to the `哦` variable, because Terse will autofill it with the first input.

### Tiers
Tiers are the reason that Terse is terse. Each non-unary function has 4 different forms, or "tiers". The base tier is called the unlimited tier of a function. The other 3 tiers are tiers zero, one, and two. All the different tiers only affect parsing, they do not affect the computation of a function.

Tiers dictate how many tokens can come after them are considered as arguments. For example, `了` is the tier one variant of `该`.

Let's translate this into a C-style language. In those languages, you call functions like so:
```
caller.func(argument)
```

But in Terse, you don't need the brackets, or the '.'. But how does Terse know when to end the function call? In C-style languages, the parentheses denotes the end of the argument list. But Terse doesn't need to have brackets?!

That is what the tier system is for. A tier-zero function is like the closing parentheses is placed right after the starting one. A tier-one function allows one token between its start and end. A tier-two allows two.

What if you need more tokens?

That is where the tier-unlimited form comes in. For a tier-unlimited form function, however, you do need to supply the closing bracket. Here are the list of brackets:

```
)      Close one function    (equals ")")
}      Close two functions   (equals "))")
）      Close three functions (equals ")))")
】      Close all open functions
```

## Structure of a Program
A program is made of a series of expressions. Expressions, are made of a starting value (atom) and a series of function calls.

Expressions can also be followed by "?", followed by 2 more expressions. This is an if statement. Looping in Terse is either done by mapping a string or list, or with the `如` function of numbers, which creates a range [0..N) and maps it with a given lambda.

## This language sounds confusing...
Don't worry if you don't get it, it's not your fault! This language was not made for practicality, it was made to be the shortest at solving problems!
