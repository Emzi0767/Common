# Contributing to Emzi's Common Code Repository
While I am not unwilling to accept contributions, the criteria for inclusion in this repository are far stricter than 
in my other projects. If you still think your code would make a valuable contribution, make sure all the following 
criteria are met.

# Solid use case
This code is something that I personally use across my various projects. I will generally not include something that I 
myself wouldn't use. Therefore, for a contribution to be accepted, it needs to provide a valuable contribution to at 
least one of my projects, however greater scope is preferred.

# Proper base
When opening a PR, please make sure your branch is even with the target branch, to avoid unnecessary surprises.

This is not required if it does not introduce problems (such as duplicating commits or re-introducing reverted 
changes), or cause conflits.

# Proper titles
When opening issues, make sure the title reflects the purpose of the issue or the pull request. Prefer past tense, and 
be brief. Further description belongs inside the issue or PR.

# Descriptive changes
I require the commits describe the changes made. It can be a short description. If you fixed or resolved an open issue, 
please reference it by using the #number notation.

Examples of good commit messages:

* `Fixed a potential memory leak with cache entities. Fixes #142.`
* `Implemented new command extension. Resolves #169.`
* `Changed message cache behaviour. It's now global instead of per-channel.`
* `Fixed a potential NRE.`
* ```
  Changed message cache behaviour:
  
  - Messages are now stored globally.
  - Cache now deletes messages when they are deleted from Discord.
  - Cache itself is now a ring buffer.
  ```

Examples of bad commit messages:

* `I a bad.`
* `Tit and tat.`
* `Fixed.`
* `Oops.`

# Code style
I use [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) 
throughout the repository, with one exception: I use `this` to explicitly denote instance member access. 

Please use modern C# features (such as expression syntax, initializer syntax, inline `out` variables, etc.) where 
possible.

# Code changes
I require that all code change commits must build successfully. This is verified by the CI. When you open a pull 
request, AppVeyor will start a build. You can view its summary by visiting it from the checks section on the PR 
overview page.

PRs that do not build will not be accepted.

In the event your code change is a style change, XML doc change, or otherwise does not change how the code works, tag 
the commit with `[ci skip]`.

# Non-code changes
In the event you change something outside of code (i.e. a meta-change or documentation), you must tag your commit with 
`[ci skip]`.
