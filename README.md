# Holo

Holo is a scripting language inspired by C#, C++, Lua and Ruby. Its design philosophy:
- Consistency
- Embeddability
- Flexibility

## Examples

### FizzBuzz

```holo
null fizzbuzz(int n) {
    for (i in 1 to 10) {
        if (i % 3 == 0 and i % 5 == 0) {
            log["FizzBuzz"]
        }
        else if (i % 3 == 0) {
            log["Fizz"]
        }
        else if (i % 5 == 0) {
            log["Buzz"]
        }
        else {
            log[i]
        }
    }
}
```

### Fibonacci Sequence

```holo
int fibonacci(int n) {
    if (n in [0, 1]) return n
    return fibonacci[n - 1] + fibonacci[n - 2]
}
log[fibonacci[10]]
```

## Design Proposal

### Overview

Every data type in Holo is a "variant" ("var").
Variants contain private variables and public methods.

## Garbage Collection

Holo runs a simple "mark & sweep" garbage collection on each actor using collaborative multi-threading:
- Each object has a state: pending, unmarked, marked.
- When an object is created, add it to a list of objects.
- When collecting garbage, set the state of each object to unmarked.
- Start at the actor, search variables recursively for objects and set their state to marked.
- Free each object whose state is unmarked.

The algorithm yields before processing each object, which should remove any pauses.

## Operators

Every operator is converted to a method call which can be overridden.

| Operator | Name | Example | Equivalent | Output | Implementation |
| --- | --- | --- | --- | --- | --- |
| `not` | logical NOT | `not true` | ``true.`not`()`` | `false` | Checks for `false` |
| `and` | logical AND | `true and false` | ``true.`and`(false)`` | `false` | Checks both are `true` |
| `or` | logical OR | `true or false` | ``true.`or`(false)`` | `true` | Checks at least one is `true` |
| `~` | bitwise XOR | `true ~ false` | ``true.`~`(false)`` | `true` | Checks exactly one is `true`, evaluating both |
| `&` | bitwise AND | `true & false` | ``true.`&`(false)`` | `false` | Checks both are `true`, evaluating both |
| `\|` | bitwise OR | `true \| false` | ``true.`\|`(false)`` | `true` | Checks at least one is `true`, evaluating both |
| `>>` | bitwise RIGHT SHIFT | `10 >> 2` | ``10.`>>`(2)`` | `2` | Multiplies by `2**x` |
| `<<` | bitwise LEFT SHIFT | `10 << 2` | ``10.`<<`(2)`` | `40` | Divides by `2**x` |
| `~` | bitwise NOT | `~10` | ``10.`~`()`` | `-11` | Negates the number and subtracts `1` |
| `==` | equals | `true == false` | ``true.`==`(false)`` | `false` | Checks for equality |
| `~=` | not equals | `true ~= false` | ``true.`~=`(false)`` | `true` | Checks for no equality |
| `>` | greater than | `10 > 2` | ``10.`>`(2)`` | `true` | Checks for greater sort order |
| `<` | less than | `10 < 2` | ``10.`<`(2)`` | `false` | Checks for lesser sort order |
| `>=` | greater than or equal to | `10 >= 2` | ``10.`>=`(2)`` | `true` | Checks for greater or equal sort order |
| `<=` | less than or equal to | `10 <= 2` | ``10.`<=`(2)`` | `false` | Checks for lesser or equal sort order |
| `-` | unary minus | `-10` | ``10.`-`()`` | `-10` | Negates the value |
| `+` | unary plus | `+10` | ``10.`+`()`` | `10` | Keeps the value |
| `+` | add | `10 + 2` | ``10.`+`(2)`` | `12` | Calculates `a plus b` |
| `-` | subtract | `10 - 2` | ``10.`-`(2)`` | `8` | Calculates `a minus b` |
| `*` | multiply | `10 * 2` | ``10.`*`(2)`` | `20` | Calculates `a multiplied by b` |
| `/` | divide | `10 / 2` | ``10.`/`(2)`` | `5` | Calculates `a divided by b` |
| `%` | modulo | `10 % 2` | ``10.`%`(2)`` | `0` | Calculates the remainder of `a divided by b` |
| `**` | exponentiate | `10 ** 2` | ``10.`**`(2)`` | `100` | Calculates `a to the power of b` |
| `in` | contained in | `0 in [1, 2, 3]` | ``[1, 2, 3].contains(0)`` | `false` | Checks if `b contains a` |
| `not_in` | not contained in | `0 in [1, 2, 3]` | ``not [1, 2, 3].contains(0)`` | `false` | Checks if `b doesn't contain a` |
| `is` | includes | `0 is int` | ``0.includes(int)`` | `true` | Checks if `a includes b` |
| `is_not` | includes | `0 is_not int` | ``not 0.includes(int)`` | `false` | Checks if `a doesn't include b` |
| `??` | null coalesce | `3 ?? 5` | ``3 == null then 5 else 3`` | `3` | Selects the first non-`null` value |
| `?.` | null propagation | `3?.stringify()` | ``3 == null then null else 3.stringify()`` | `true` | Calls the method if not `null` |

Compound assignment operators are shorthand for applying operators to the current value.

| Operator | Equivalent |
| --- | --- |
| `name += value` | `name = name + value` |
| `name -= value` | `name = name - value` |
| `name *= value` | `name = name * value` |
| `name /= value` | `name = name / value` |
| `name %= value` | `name = name % value` |
| `name **= value` | `name = name ** value` |
| `name ??= value` | `name = name ?? value` |