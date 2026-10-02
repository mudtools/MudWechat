// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.Work.Callback.Events;

/// <summary>
/// 回调事件强类型解析器（v1 方案 §5.5）：将 <see cref="WechatCallbackEvent.DecryptedXml"/> 明文
/// 解析为逐事件 DTO（XDocument 手动映射，无反射，AOT 安全）。
/// </summary>
/// <remarks>
/// <para>
/// 事件键（<see cref="WechatCallbackEvent.EventTypeKey"/>）不匹配、明文缺失或明文非 XML 时返回 <c>null</c>
/// （不抛异常）。DTO 字段全部可空——通讯录回调存在「仅回调子集 + 敏感字段降权」的事件级权限语义，
/// 处理器<b>不得假设必有值</b>。
/// </para>
/// </remarks>
public static class WechatCallbackEventParser
{
    /// <summary>解析新增成员事件（<c>create_user</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static UserCreatedEvent? ParseUserCreated(WechatCallbackEvent evt)
    {
        if (!IsChangeType(evt, WechatCallbackEventTypes.CreateUser, out var root))
        {
            return null;
        }

        var dto = new UserCreatedEvent
        {
            UserID = Text(root, "UserID"),
            Name = Text(root, "Name"),
            Department = Text(root, "Department"),
            MainDepartment = Text(root, "MainDepartment"),
            IsLeaderInDept = Text(root, "IsLeaderInDept"),
            DirectLeader = Text(root, "DirectLeader"),
            Position = Text(root, "Position"),
            Mobile = Text(root, "Mobile"),
            Gender = Text(root, "Gender"),
            Email = Text(root, "Email"),
            BizMail = Text(root, "BizMail"),
            Status = Text(root, "Status"),
            Avatar = Text(root, "Avatar"),
            Alias = Text(root, "Alias"),
            Telephone = Text(root, "Telephone"),
            Address = Text(root, "Address"),
        };

        foreach (var item in root.Element("ExtAttr")?.Elements("Item") ?? Enumerable.Empty<XElement>())
        {
            dto.ExtAttr.Add(new WechatCallbackExtAttrItem
            {
                Name = (string?)item.Attribute("Name"),
                Type = (string?)item.Attribute("Type"),
                Value = item.Element("Text")?.Value,
            });
        }

        return dto;
    }

    /// <summary>解析更新成员事件（<c>update_user</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static UserUpdatedEvent? ParseUserUpdated(WechatCallbackEvent evt)
    {
        if (!IsChangeType(evt, WechatCallbackEventTypes.UpdateUser, out var root))
        {
            return null;
        }

        var dto = new UserUpdatedEvent
        {
            UserID = Text(root, "UserID"),
            NewUserID = Text(root, "NewUserID"),
            Name = Text(root, "Name"),
            Department = Text(root, "Department"),
            MainDepartment = Text(root, "MainDepartment"),
            IsLeaderInDept = Text(root, "IsLeaderInDept"),
            DirectLeader = Text(root, "DirectLeader"),
            Position = Text(root, "Position"),
            Mobile = Text(root, "Mobile"),
            Gender = Text(root, "Gender"),
            Email = Text(root, "Email"),
            BizMail = Text(root, "BizMail"),
            Status = Text(root, "Status"),
            Avatar = Text(root, "Avatar"),
            Alias = Text(root, "Alias"),
            Telephone = Text(root, "Telephone"),
            Address = Text(root, "Address"),
        };

        foreach (var item in root.Element("ExtAttr")?.Elements("Item") ?? Enumerable.Empty<XElement>())
        {
            dto.ExtAttr.Add(new WechatCallbackExtAttrItem
            {
                Name = (string?)item.Attribute("Name"),
                Type = (string?)item.Attribute("Type"),
                Value = item.Element("Text")?.Value,
            });
        }

        return dto;
    }

    /// <summary>解析删除成员事件（<c>delete_user</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static UserDeletedEvent? ParseUserDeleted(WechatCallbackEvent evt)
        => IsChangeType(evt, WechatCallbackEventTypes.DeleteUser, out var root)
            ? new UserDeletedEvent { UserID = Text(root, "UserID") }
            : null;

    /// <summary>解析新增部门事件（<c>create_party</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static PartyCreatedEvent? ParsePartyCreated(WechatCallbackEvent evt)
        => IsChangeType(evt, WechatCallbackEventTypes.CreateParty, out var root)
            ? new PartyCreatedEvent
            {
                Id = Text(root, "Id"),
                Name = Text(root, "Name"),
                ParentId = Text(root, "ParentId"),
                Order = Text(root, "Order"),
            }
            : null;

    /// <summary>解析更新部门事件（<c>update_party</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static PartyUpdatedEvent? ParsePartyUpdated(WechatCallbackEvent evt)
        => IsChangeType(evt, WechatCallbackEventTypes.UpdateParty, out var root)
            ? new PartyUpdatedEvent
            {
                Id = Text(root, "Id"),
                Name = Text(root, "Name"),
                ParentId = Text(root, "ParentId"),
            }
            : null;

    /// <summary>解析删除部门事件（<c>delete_party</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static PartyDeletedEvent? ParsePartyDeleted(WechatCallbackEvent evt)
        => IsChangeType(evt, WechatCallbackEventTypes.DeleteParty, out var root)
            ? new PartyDeletedEvent { Id = Text(root, "Id") }
            : null;

    /// <summary>解析标签成员变更事件（<c>update_tag</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static TagUpdatedEvent? ParseTagUpdated(WechatCallbackEvent evt)
        => IsChangeType(evt, WechatCallbackEventTypes.UpdateTag, out var root)
            ? new TagUpdatedEvent
            {
                TagId = Text(root, "TagId"),
                AddUserItems = Text(root, "AddUserItems"),
                DelUserItems = Text(root, "DelUserItems"),
                AddPartyItems = Text(root, "AddPartyItems"),
                DelPartyItems = Text(root, "DelPartyItems"),
            }
            : null;

    /// <summary>解析异步任务完成事件（<c>batch_job_result</c>；事件键不匹配返回 <c>null</c>）。</summary>
    public static BatchJobResultEvent? ParseBatchJobResult(WechatCallbackEvent evt)
    {
        if (evt == null || !evt.IsBatchJobResult || string.IsNullOrEmpty(evt.DecryptedXml))
        {
            return null;
        }

        try
        {
            var root = XDocument.Parse(evt.DecryptedXml).Root;
            if (root == null)
            {
                return null;
            }

            return new BatchJobResultEvent
            {
                JobId = Text(root, "JobId"),
                JobType = Text(root, "JobType"),
                ErrCode = Text(root, "ErrCode"),
                ErrMsg = Text(root, "ErrMsg"),
            };
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    /// <summary>解析逗号分隔的 id 串（如 <c>Department</c> 的 <c>"1,2,3"</c>）；非法片段跳过。</summary>
    public static List<long> ParseIdList(string? raw)
    {
        var ids = new List<long>();
        if (string.IsNullOrEmpty(raw))
        {
            return ids;
        }

        // `!`：netstandard2.0 的 string.IsNullOrEmpty 无 NotNullWhen 标注，流分析无法收窄（已在上方判空）。
        foreach (var part in raw!.Split(','))
        {
            if (long.TryParse(part.Trim(), out var id))
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    /// <summary>解析分隔符串（如 <c>DirectLeader</c> 的竖线分隔、<c>AddUserItems</c> 的逗号分隔）；空白片段跳过。</summary>
    public static List<string> ParseTextList(string? raw, char separator)
    {
        var items = new List<string>();
        if (string.IsNullOrEmpty(raw))
        {
            return items;
        }

        // `!`：netstandard2.0 的 string.IsNullOrEmpty 无 NotNullWhen 标注，流分析无法收窄（已在上方判空）。
        foreach (var part in raw!.Split(separator))
        {
            if (part.Trim().Length > 0)
            {
                items.Add(part.Trim());
            }
        }

        return items;
    }

    /// <summary>
    /// 事件键匹配判定：必须是通讯录变更事件且 <see cref="WechatCallbackEvent.ChangeType"/> 精确命中；
    /// 命中时输出明文根节点。
    /// </summary>
    private static bool IsChangeType(WechatCallbackEvent? evt, string changeType, out XElement root)
    {
        root = null!;
        if (evt == null || !evt.IsChangeContact ||
            !string.Equals(evt.ChangeType, changeType, StringComparison.Ordinal) ||
            string.IsNullOrEmpty(evt.DecryptedXml))
        {
            return false;
        }

        try
        {
            var parsed = XDocument.Parse(evt.DecryptedXml).Root;
            if (parsed == null)
            {
                return false;
            }

            root = parsed;
            return true;
        }
        catch (System.Xml.XmlException)
        {
            return false;
        }
    }

    /// <summary>读取根节点的直接子元素文本（CDTA/转义已由 XElement.Value 归一）。</summary>
    private static string? Text(XElement root, string name) => root.Element(name)?.Value;
}
