using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Notes.Application.Notes.Commands.UpdateNote
{
    public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
    {
        public UpdateNoteCommandValidator()
        {
            RuleFor(createNoteCommand => createNoteCommand.Title).NotEmpty().MaximumLength(250);
            RuleFor(createNoteCommand => createNoteCommand.UserId).NotEqual(Guid.Empty);
            RuleFor(createNoteCommand => createNoteCommand.Id).NotEqual(Guid.Empty);
        }
    }
}
