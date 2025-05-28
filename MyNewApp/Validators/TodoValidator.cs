using System.Security;
using FluentValidation;
using MyNewApp.Models;

namespace MyNewApp.Validators
{
    public class TodoValidator : AbstractValidator<Todo>
    {
        public TodoValidator()
        {
            RuleSet("Create", () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required.");
                RuleFor(x => x.DueDate)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");
                RuleFor(x => x.IsCompleted)
                    .Equal(false).WithMessage("Cannot create a completed todo.");
            });

            RuleSet("Update", () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required on update.");
                RuleFor(x => x.DueDate)
                    .NotEmpty().WithMessage("Due date is required on update.");
                RuleFor(x => x.IsCompleted)
                    .NotEmpty().WithMessage("IsCompleted is required on update.");
            });
        } 
    }
}