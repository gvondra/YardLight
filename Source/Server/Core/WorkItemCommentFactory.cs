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

        private WorkItemComment Create(WorkItemCommentData data, IWorkItem workItem) => new WorkItemComment(data, _dataSaver, workItem);

        public IWorkItemComment Create(IWorkItem workItem, string text, WorkItemCommentType workItemCommentType)
        {
            if (workItemCommentType == WorkItemCommentType.NotSet)
                throw new ArgumentNullException(nameof(workItemCommentType));
            return Create(new WorkItemCommentData
            {
                Text = text ?? string.Empty,
                Type = (short) workItemCommentType
            },
            workItem);
        }

        public async Task<IEnumerable<IWorkItemComment>> GetByWorkItem(ISettings settings, IWorkItem workItem)
        {
            return (await _dataFactory.GetByWorkItemId(new DataSettings(settings), workItem.WorkItemId))
                .Select<WorkItemCommentData, IWorkItemComment>(d => Create(d, workItem));
        }
    }
}
