﻿using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SsbHedger.MediatorCommands
{
    public class UpdateConfigurationMediatorCommandHandler : IRequestHandler<UpdateConfigurationMediatorCommand>
    {
        public Task<Unit> Handle(UpdateConfigurationMediatorCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
