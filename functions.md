# Unary

----

## Number

-------------------
| Symbol | Description | Output Type |
---------|-------------|-------------|
| 经 | Stores the current value into the assignable variable "间" | Number |
| 妈 | Makes the first autofill in this scope the current value | Number |
| 用 | Return length of number, inc. decimal point | Number |
| 打 | Negate the number | Number |
| 地 | Double the number | Number |
| 再 | Halve (not rounded) | Number |
| 因 | Increment | Number |
| 呢 | Decrement | Number |
| 女 | Convert to string | String |
| 告 | Convert to base-10 digit array | List |
| 最 | Convert to hexadecimal string | Number |
| 手 | Convert to binary string | Number |
| 前 | 1 if 0, else 0 | Number |
| 找 | Absolute value | Number |
| 行 | Factorial (Note: Returns the caller if caller is negative) | Number |
| 快 | Square root | Number |
| 而 | Square | Number |
| 先 | Range \[1..x\] (inclusive) | List |
| 像 | Binary complement | Number |
| 等 | Number parity | Number |
| 被 | Round up | Number |
| 从 | Round down | Number |
| 明 | Sign | Number |
| 中 | To character | String |

## String

-------------------
| Symbol | Description | Output Type |
---------|-------------|-------------|
| 经 | Stores the current expression result into the assignable variable "间" | String |
| 妈 | Makes the first autofill in this scope the current value | String |
| 用 | Length | Number |
| 打 | Reverse | String |
| 地 | 1st char | String |
| 再 | Last char | String |
| 因 | To number (returns caller string if not parsable) | Number/String |
| 呢 | To char array | List[String] |
| 女 | Split on " " | List[String] |
| 告 | Split on newline "\n" | String |
| 最 | Array of char values, or just an int if length 1 | List[Number]/Number |
| 手 | All permutations of the string | List[String] |
| 前 | To Upper | String |
| 找 | To Lower | String |
| 行 | Repeat itself, twice | String |
| 快 | Split on newlines, then transpose rows and columns, then rejoin with newlines | String |
| 而 | Unique chars in order they appear | String |
| 死 | All subsections | List[String] |
| 先 |  | String |
| 像 |  | String |
| 等 |  | String |
| 被 |  | String |
| 从 |  | String |
| 明 |  | String |
| 中 |  | String |

## List

-------------------
| Symbol | Description | Output Type |
---------|-------------|-------------|
| 经 | Stores the current expression result into the assignable variable "间" | List |
| 妈 | Makes the first autofill in this scope the current value | List |
| 用 | Length | Number |
| 打 | Reverse the list | List |
| 地 | 1st element | T |
| 再 | Last element | T |
| 因 | Sort | List |
| 呢 | Concatenate | List |
| 女 | Join w/ " " | String |
| 告 | Join w/ "\n" | String |
| 最 | Sum (Map strings by parsing them or 0, lists to 0) | Humber |
| 手 | Product (Map strings by parsing them or 1, lists to 1) | Number |
| 前 | Flatten Completely | List |
| 找 | Remove first | List |
| 行 | Max (Strings to parsed or NegativeInfinity, lists to NegativeInfinity) | List |
| 快 | Min (Strings to parsed or PositiveInfinity, lists to PositiveInfinity) | Number |
| 而 | Cumulative Sum (Strings to parsed or 0, lists to 0) | Number |
| 死 | Subsections (inc. self) | List |
| 先 | Permutations | List |
| 像 | Combinations | List |
| 等 | Group | List |
| 被 |  | List |
| 从 |  | List |
| 明 |  | List |
| 中 |  | List |

# Binary

----

## Number

-------------------
| Symbol | Argument Type | Description | Output Type |
---------|---------------|-------------|-------------|
| 和 | Number | Addition | Number |
| 和 | String | Append s to N | String |
| 和 | List | Prepend N to l | List |
| 真 | Number | Subtraction | Number |
| 真 | Number | Multiplication | Number |
| 现 | Number | Division | Number |
| 做 | Number | Range [x..y] | List |
| 大 | Number | Modulus | Number |
| 啊 | Number | Round up to nearest multiple of n | Number |
| 怎 | Number | Round down to nearest multiple of n | Number |
| 出 | Number | To nth power of 10, multplied by N | Number |
| 点 | Number | Compare: N == n -> 0, N > n -> 1, N < n -> -1 | Number |
| 起 | Number | Coprime N to n | Number |
| 天 | Number | Smaller of caller and argument | Number |
| 把 | Number | Larger of N and n | Number |
| 开 | Number | n - N (subtraction but reversed) | Number |
| 让 | Number | N to nth power | Number |
| 给 | Number | nth root of N | Number |
| 但 | Number | Round to nearest multiple of n | Number |
| 谢 | Number | Base n string | String |
| 谢 | String | Base s string | String |
| 谢 | String | Base s string | String |
| 着 | Number | DivRem | List |
| 只 | Lambda[Number] | First integer greater than or equal to x that fulfills the lambda | Number |
| 些 | String | Get Nth character of s | String |
| 些 | List | Get Nth character of l | T |
| 如 | Lambda[Number] | Create range [0..N), then map it with Lambda | List |
| 家 | Number |  | Number |
| 后 | Number |  | Number |
| 儿 | Number |  | Number |
| 多 | Number |  | Number |
| 意 | Number |  | Number |
| 别 | Number |  | Number |
| 所 | Number |  | Number |
| 话 | Number |  | Number |
| 小 | Number |  | Number |
| 自 | Number |  | Number |
| 回 | Number |  | Number |
| 然 | Number |  | Number |
| 果 | Number |  | Number |
| 发 | Number |  | Number |
| 见 | Number |  | Number |
| 心 | Number |  | Number |
| 走 | Number |  | Number |
| 定 | Number |  | Number |
| 听 | Number |  | Number |
| 觉 | Number |  | Number |
| 太 | Number |  | Number |
| 该 | Number | Pair | List |
| 该 | String | Pair | List |
| 该 | List | Pair | List |
| 当 | Number | Equals | Number |
| 当 | String | Equals | Number |
| 当 | List | Equals | Number |

## String

-------------------
| Symbol | Argument Type | Description | Output Type |
---------|---------------|-------------|-------------|
| 和 | String | Concatenation | String |
| 和 | Number | Concatenation | String |
| 和 | Number | Prepend S to l | String |
| 下 | String | Split S on s | List |
| 下 | Number | Split S on n | List |
| 真 | String | Prepend S with s | String |
| 现 | Number | Split S into chunks of size n | List |
| 做 | Number | Split S into n chunks containing consecutive elements | List |
| 大 | String | First index of s within S | Number |
| 啊 | String | Last index of s within S | Number |
| 怎 | String | All indexes of s within S | List |
| 怎 | String | All indexes of s within S | List |
| 出 | String | Delete all instances of s from S | String |
| 出 | List | For each pair of strings (s1, s2) in l, replace each instance of s1 in S with s2. If there are odd number of elements in l, for the last element it is replaced with empty string | String |
| 出 | string | Delete all instances of s from S | String |
| 出 | string | For each pair of strings (s1, s2) in l, replace each instance of s1 in S with s2. If there are odd number of elements in l, for the last element it is replaced with empty string | String |
| 点 | Lambda[T,Number] | Map | List |
| 起 | Lambda[T,Number] | Filter | String |
| 天 | Lambda[T,Number] | Group By | List[List[String]] |
| 把 | Lambda[String,Number] | Filter out truthy | String |
| 开 | Lambda[String,Number] | Reduce | List |
| 让 | Lambda[T,Number] | Culmulative Reduction | List |
| 给 | String | Sum with lambda | Number |
| 但 | Lambda[T,Number] | Cumulative Sum with lambda | List |
| 谢 | Number | Take first n elements of S | List |
| 着 | Number | Take last n elements of S | List |
| 只 | Lambda[String,Number] | Take while lambda returns true | String |
| 些 | Number | Skip first n elements of S | String |
| 如 | Number | Skip last n elements of S | String |
| 家 | Lambda[T,Number] | Skip while lambda returns true | String |
| 后 | Lambda[T,Number] | Sort with lambda | List |
| 儿 | Lambda[T,Number] | Sort descending with lambda | List |
| 多 | Number | Get nth char (negative gets from back) | String |
| 多 | List | Foreach value in l, get nth element (negative gets from back) | List |
| 意 | Number | Contains the certain VObject (in this case number) | Number |
| 意 | String | Contains the certain VObject (in this case string) | Number |
| 别 | String | Remove all characters in s from S | String |
| 别 | Number | Remove all characters in n from S | String |
| 所 | Number | Convert S from base-n string to base-10 integer, else returns itself if invalid (supports base 2 - 36) | String/Number |
| 所 | String | Convert S from base-s string to base-10 integer, else returns itself if invalid | String/Number |
| 话 | String | Remove all | String |
| 小 | String |  | String |
| 自 | String |  | String |
| 回 | String |  | String |
| 然 | String |  | String |
| 果 | String |  | String |
| 发 | String |  | String |
| 见 | String |  | String |
| 心 | String |  | String |
| 走 | String |  | String |
| 定 | String |  | String |
| 听 | String |  | String |
| 觉 | String |  | String |
| 太 | String |  | String |
| 该 | String | Pair | List |
| 该 | Number | Pair | List |
| 该 | List | Pair | List |
| 当 | List | Equals | Number |
| 当 | String | Equals | Number |
| 当 | Number | Equals | Number |

## List

-------------------
| Symbol | Argument Type | Description | Output Type |
---------|---------------|-------------|-------------|
| 和 | List | Concatenation | List |
| 和 | String | Append s to L | List |
| 和 | Number | Append n to L | List |
| 下 | String | Join L on s | String |
| 下 | Number | Join L on n | List |
| 真 | List | Prepend l to L | List |
| 现 | Number | Split S into chunks of size n | List |
| 做 | List | Split S into n chunks | List |
| 大 | List | First index of s within S | List |
| 啊 | List | Last index of s within S | List |
| 怎 | List | All indexes of s within S | List |
| 怎 | String | All indexes of s within S | List |
| 怎 | List | All indexes of s within S | List |
| 出 | String | Delete all instances of s from L | List |
| 出 | Number | Delete all instances of s from L | List |
| 点 | Lambda[T,Number] | Map | List |
| 起 | Lambda[T,Number] | Filter | List |
| 天 | Lambda[T,Number] | Group By | List |
| 把 | Lambda[T,Number] | Filter out truthy | List |
| 开 | Lambda[T,Number] | Reduce | List |
| 让 | Lambda[T,Number] | Culmulative Reduction | List |
| 给 | Lambda[T,Number] | Sum with lambda | Number |
| 但 | Lambda[T,Number] | Cumulative Sum with lambda | List |
| 谢 | Number | Take first n elements of L | List |
| 着 | Number | Take last n elements of L | List |
| 只 | Lambda[T,Number] | Take while lambda returns true | List |
| 些 | Number | Skip first n elements of L | List |
| 如 | Number | Skip last n elements of L | List |
| 家 | Lambda[T,Number] | Skip while lambda returns true | List |
| 后 | Lambda[T,Number] | Sort with lambda | List |
| 儿 | Lambda[T,Number] | Sort descending with lambda | List |
| 多 | Number | Get nth element (negative gets from back) | T |
| 多 | List | Foreach value in l, get nth element (negative gets from back) | List |
| 意 | List | Contains the certain VObject (in this case list) | Number |
| 意 | Number | Contains the certain VObject (in this case number) | Number |
| 意 | String | Contains the certain VObject (in this case string) | Number |
| 别 | Lambda[T, Number] | Given that L is of the structure [l, n] or [n,l], repeatedly pass the last element into the lambda and append it onto l until l is of n length, then return the nth element of l | T |
| 所 | Lambda[T, Number] | Given that L is of the structure [l, n] or [n,l], repeatedly pass the last element into the lambda and append it onto l until l is of n length, then return the first n elements of l | T |
| 话 | List |  | List |
| 小 | List |  | List |
| 自 | List |  | List |
| 回 | List |  | List |
| 然 | List |  | List |
| 果 | List |  | List |
| 发 | List |  | List |
| 见 | List |  | List |
| 心 | List |  | List |
| 走 | List |  | List |
| 定 | List |  | List |
| 听 | List |  | List |
| 觉 | List |  | List |
| 太 | List |  | List |
| 该 | List | Pair | List |
| 该 | Number | Pair | List |
| 该 | String | Pair | List |
| 当 | List | Equals | Number |
| 当 | String | Equals | Number |
| 当 | Number | Equals | Number |
