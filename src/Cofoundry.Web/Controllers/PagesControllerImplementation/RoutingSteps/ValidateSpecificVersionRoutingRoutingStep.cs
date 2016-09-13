﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// If the request is for a specific page version, we validate that the user has permission to see 
    /// that version and that the version requested is valid. If it is not valid then the version
    /// parameters are discarded.
    /// </summary>
    public class ValidateSpecificVersionRoutingRoutingStep : IValidateSpecificVersionRoutingRoutingStep
    {
        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // Ensure that non-authenticated users can't access previous versions
            if (state.SiteViewerMode != SiteViewerMode.SpecificVersion)
            {
                state.InputParameters.VersionId = null;
            }
            else if (state.PageRoutingInfo != null)
            {
                var versionRoute = state.PageRoutingInfo.GetVersionRoute(
                    state.InputParameters.IsEditingCustomEntity,
                    state.SiteViewerMode.ToWorkFlowStatusQuery(),
                    state.InputParameters.VersionId);

                // If this isn't an old version of a page, set the SiteViewerMode accordingly.
                if (versionRoute != null)
                {
                    switch (versionRoute.WorkFlowStatus)
                    {
                        case Cofoundry.Domain.WorkFlowStatus.Draft:
                            state.SiteViewerMode = SiteViewerMode.Draft;
                            break;
                        case Cofoundry.Domain.WorkFlowStatus.Published:
                            state.SiteViewerMode = SiteViewerMode.Live;
                            break;
                    }
                }
                else
                {
                    // Could not find a version, id must be invalid
                    state.InputParameters.VersionId = null;
                }
            }

            return Task.FromResult(true);
        }
    }
}