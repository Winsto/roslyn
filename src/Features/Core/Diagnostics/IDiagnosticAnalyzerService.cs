﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal interface IDiagnosticAnalyzerService
    {
        /// <summary>
        /// re-analyze given projects and documents
        /// </summary>
        void Reanalyze(Workspace workspace, IEnumerable<ProjectId> projectIds = null, IEnumerable<DocumentId> documentIds = null);

        /// <summary>
        /// get specific diagnostics currently stored in the source. returned diagnostic might be out-of-date if solution has changed but analyzer hasn't run for the new solution.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetSpecificCachedDiagnosticsAsync(Workspace workspace, object id, CancellationToken cancellationToken);

        /// <summary>
        /// get diagnostics currently stored in the source. returned diagnostic might be out-of-date if solution has changed but analyzer hasn't run for the new solution.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetCachedDiagnosticsAsync(Workspace workspace, ProjectId projectId = null, DocumentId documentId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// get specific diagnostics for the given solution. all diagnostics returned should be up-to-date with respect to the given solution.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetSpecificDiagnosticsAsync(Solution solution, object id, CancellationToken cancellationToken);

        /// <summary>
        /// get diagnostics for the given solution. all diagnostics returned should be up-to-date with respect to the given solution.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetDiagnosticsAsync(Solution solution, ProjectId projectId = null, DocumentId documentId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// get diagnostics of the given diagnostic ids from the given solution. all diagnostics returned should be up-to-date with respect to the given solution.
        /// Note that for project case, this metHod returns diagnostics from all project documents as well. Use <see cref="GetProjectDiagnosticsForIdsAsync(Solution, ProjectId, ImmutableHashSet{string}, CancellationToken)"/>
        /// if you want to fetch only project diagnostics without source locations.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetDiagnosticsForIdsAsync(Solution solution, ProjectId projectId = null, DocumentId documentId = null, ImmutableHashSet<string> diagnosticIds = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// get project diagnostics (diagnostics with no source location) of the given diagnostic ids from the given solution. all diagnostics returned should be up-to-date with respect to the given solution.
        /// Note that this method doesn't return any document diagnostics. Use <see cref="GetDiagnosticsForIdsAsync(Solution, ProjectId, DocumentId, ImmutableHashSet{string}, CancellationToken)"/> to also fetch those.
        /// </summary>
        Task<ImmutableArray<DiagnosticData>> GetProjectDiagnosticsForIdsAsync(Solution solution, ProjectId projectId = null, ImmutableHashSet<string> diagnosticIds = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// try to return up to date diagnostics for the given span for the document.
        /// 
        /// it will return true if it was able to return all up-to-date diagnostics.
        ///  otherwise, false indicating there are some missing diagnostics in the diagnostic list
        /// </summary>
        Task<bool> TryGetDiagnosticsForSpanAsync(Document document, TextSpan range, List<DiagnosticData> diagnostics, CancellationToken cancellationToken);

        /// <summary>
        /// return up to date diagnostics for the given span for the document
        /// 
        /// this can be expensive since it is force analyzing diagnostics if it doesn't have up-to-date one yet.
        /// </summary>
        Task<IEnumerable<DiagnosticData>> GetDiagnosticsForSpanAsync(Document document, TextSpan range, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of the diagnostics that are provided by this service.
        /// If the given <paramref name="projectOpt"/> is non-null, then gets the diagnostics for the project.
        /// Otherwise, returns the global set of diagnostics enabled for the workspace.
        /// </summary>
        /// <returns>A mapping from analyzer name to the diagnostics produced by that analyzer</returns>
        ImmutableDictionary<string, ImmutableArray<DiagnosticDescriptor>> GetDiagnosticDescriptors(Project projectOpt);
    }
}
