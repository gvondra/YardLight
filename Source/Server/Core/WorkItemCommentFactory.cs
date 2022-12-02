using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardLight.CommonCore;
using YardLight.Data.Framework;
using YardLight.Data.Models;
using YardLight.Framework;
using YardLight.Framework.Enumerations;

namespace YardLight.Core
{
    public class WorkItemCommentFactory : IWorkItemCommentFactory
    {
        private readonly IWorkItemCommentDataFactory _dataFactory;
        private readonly IWorkItemCommentDataSaver _dataSaver;

        public WorkItemCommentFactory(IWorkItemCommentDataFactory dataFactory, IWorkItemCommentDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private WorkItemComment Create(WorkItemCommentData data) => new WorkItemComment(data, _dataSaver);

        public IWorkItemComment Create(Guid workItemId, string text, WorkItemCommentType workItemCommentType)
        {
            if (workItemCommentType == WorkItemCommentType.NotSet)
                throw new ArgumentNullException(nameof(workItemCommentType));
            return Create(new WorkItemCommentData
            {
                Text = text ?? string.Empty,
                Type = (short) workItemCommentType,
                WorkItemId = workItemId
            });
        }

        public async Task<IEnumerable<IWorkItemComment>> GetByWorkItemId(ISettings settings, Guid workItemId)
        {
            return (await _dataFactory.GetByWorkItemId(new DataSettings(settings), workItemId))
                .Select<WorkItemCommentData, IWorkItemComment>(Create);
        }
    }
}
