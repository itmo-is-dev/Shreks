﻿using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Tools;

public interface ISheetManagementService
{
    Task CreateOrClearSheetAsync(ISheet sheet, CancellationToken token);
}