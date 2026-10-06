// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Mud.HttpUtils.Payloads;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 企业微信回调载荷转换器。
/// </summary>
/// <remarks>
/// <para>
/// <b>签名契约为硬约束</b>：方法必须 <c>static</c>、非泛型（除自身类型参数外）、参数形态固定 ——
/// 由上游 <c>PayloadFieldMapGenerator</c> 在编译期校验，不符即 <c>PAYLOAD004</c>。
/// 本类的方法集与上游推断表（<c>Text</c>/<c>Number&lt;T&gt;</c>/<c>Flag&lt;T&gt;</c>/<c>Delimited&lt;T&gt;</c>/
/// <c>Items&lt;T&gt;</c>/<c>ItemsWithAttributes&lt;T&gt;</c>/<c>Object&lt;TSingle&gt;</c>/<c>ItemsObject&lt;TItem&gt;</c>）
/// 逐项对应；另有 <see cref="RepeatSiblings"/> 走特性 <c>Method</c> 显式通道
/// （上游推断表无「重复同名叶兄弟」形态，见该方法备注）。
/// </para>
/// <para>
/// <b>不变量：节点缺失 ⇒ 返回默认值，绝不抛异常</b>。这是「字段可空」语义的机器化表达：
/// 通讯录助手在 2022-08-15 后的新 URL 仅回调字段子集，缺失即 <c>null</c>（行为保持矩阵 B6/B10）。
/// </para>
/// <para>
/// <b>无状态约束（上游 §3.7 / README「使用约束」）</b>：本类<b>不得</b>持有静态可变状态，
/// 亦不得读取任何请求/租户上下文 —— 映射表是可被多线程共享的静态单例。
/// </para>
/// </remarks>
public static class WechatPayloadConverter
{
    /// <summary>标量文本；节点缺失或空 ⇒ <c>null</c>。</summary>
    public static string? Text(PayloadNode? node)
    {
        var text = node?.Value;
        return string.IsNullOrEmpty(text) ? null : text;
    }

    /// <summary>数值（官方以字符串承载，如 <c>&lt;Order&gt;10&lt;/Order&gt;</c>）；非法 ⇒ <c>null</c>。</summary>
    public static T? Number<T>(PayloadNode? node) where T : struct
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
            return null;

        if (!long.TryParse(text!.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return null;

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        try
        {
            return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (OverflowException)
        {
            return null;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    /// <summary>布尔标量（官方以 0/1 承载）；非法 ⇒ <c>null</c>。</summary>
    public static T? Flag<T>(PayloadNode? node) where T : struct
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
            return null;

        var token = text!.Trim();
        bool parsed;
        if (token == "1" || string.Equals(token, "true", StringComparison.OrdinalIgnoreCase))
            parsed = true;
        else if (token == "0" || string.Equals(token, "false", StringComparison.OrdinalIgnoreCase))
            parsed = false;
        else
            return null;

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        if (targetType != typeof(bool))
            return null;

        return (T)(object)parsed;
    }

    /// <summary>
    /// 分隔符串 → 列表（官方 <c>Department</c> 的 <c>"1,2,3"</c>、<c>DirectLeader</c> 的 <c>"a|b"</c> 形态）；
    /// 空白片段与非法片段<b>跳过</b>；节点缺失 ⇒ 空列表。
    /// </summary>
    public static List<T> Delimited<T>(PayloadNode? node, char separator)
    {
        var items = new List<T>();
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
            return items;

        foreach (var part in text!.Split(separator))
        {
            var token = part.Trim();
            if (token.Length == 0)
                continue;

            if (typeof(T) == typeof(string))
            {
                items.Add((T)(object)token);
                continue;
            }

            if (!long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                continue;

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            try
            {
                items.Add((T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture));
            }
            catch (InvalidCastException)
            {
                // 非法片段跳过（与旧 ParseIdList 语义一致）
            }
            catch (OverflowException)
            {
                // 同上
            }
        }

        return items;
    }

    /// <summary>
    /// 同构嵌套项 → 列表（官方 95796 的 <c>&lt;GroupIds&gt;&lt;GroupId&gt;5&lt;/GroupId&gt;&lt;/GroupIds&gt;</c> 形态）；
    /// 空白项跳过；节点缺失 ⇒ 空列表。
    /// </summary>
    /// <param name="node">容器节点（由 <c>[PayloadField(Element)]</c> 定位到容器本身）。</param>
    /// <param name="itemName">项元素名（<c>ItemName</c> 声明）。</param>
    public static List<T> Items<T>(PayloadNode? node, string itemName)
    {
        var items = new List<T>();
        if (node == null || string.IsNullOrEmpty(itemName))
            return items;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, itemName, StringComparison.Ordinal))
                continue;

            var value = child.Value;
            if (string.IsNullOrEmpty(value))
                continue;

            var token = value!.Trim();
            if (token.Length == 0)
                continue;

            if (typeof(T) != typeof(string))
                continue;

            items.Add((T)(object)token);
        }

        return items;
    }

    /// <summary>
    /// 重复同名叶兄弟元素 → 列表（官方 wedoc 回调的 id 列表形态：
    /// <c>&lt;DocId&gt;A&lt;/DocId&gt;&lt;DocId&gt;B&lt;/DocId&gt;</c>，<b>无包装容器</b>）；
    /// 空白项跳过；节点缺失 ⇒ 空列表。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 依赖 <c>XElementPayloadSource</c> 的同名叶兄弟合并投影：≥2 个同名叶兄弟在节点树上呈现为
    /// 「同名容器（<see cref="PayloadNode.Children"/> = 原始元素全集）」，本方法取容器子节点文本；
    /// 恰 1 个元素时未触发合并，定位到的就是叶节点本身，取其自有文本。
    /// 仅适用于<b>叶</b>兄弟重复形态——复杂节点（含子节点）的重复请走
    /// <see cref="ItemsObject{TItem}"/> 的包装容器通道。
    /// </para>
    /// </remarks>
    public static List<string> RepeatSiblings(PayloadNode? node)
    {
        var items = new List<string>();
        if (node == null)
            return items;

        if (node.Children.Count == 0)
        {
            // 单元素形态：定位节点即叶本身（未触发合并投影）。
            AddSiblingValue(items, node.Value);
            return items;
        }

        // 合并形态：同名容器的子节点即原始重复元素（保序）。
        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            AddSiblingValue(items, children[i]?.Value);
        }

        return items;
    }

    private static void AddSiblingValue(List<string> items, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        var token = value!.Trim();
        if (token.Length == 0)
            return;

        items.Add(token);
    }

    /// <summary>
    /// 重复同名复杂兄弟元素 → 契约化对象列表（官方会议「素材上传结果」的
    /// <c>&lt;UploadInfo&gt;…&lt;/UploadInfo&gt;&lt;UploadInfo&gt;…&lt;/UploadInfo&gt;</c> 形态，
    /// <b>无包装容器</b>）；节点缺失 ⇒ 空列表。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 依赖 <c>XElementPayloadSource</c> 的根层同名兄弟合并投影：≥2 个同名复杂兄弟在节点树上呈现为
    /// 「同名容器（<see cref="PayloadNode.Children"/> = 原始元素全集）」。合成容器的判别依据是
    /// <b>子节点中存在与容器同名的节点</b>（合并语义保证容器与成员同名）；恰 1 个元素时未触发合并，
    /// 定位到的就是元素本身，直接按单项绑定。
    /// </para>
    /// <para>
    /// 与 <see cref="RepeatSiblings"/>（叶列表）同理属 <c>Method</c> 显式通道；因 <c>Method</c> 方法
    /// 须非泛型，本方法绑定具体项类型 <see cref="WechatCallbackMeetingMediumUploadItem"/>
    /// （其字段映射仍由 <c>[PayloadContract]</c> 生成物承担，本方法只做「单/多形态分派」）。
    /// </para>
    /// </remarks>
    public static List<WechatCallbackMeetingMediumUploadItem> RepeatMediumUploadItems(PayloadNode? node)
    {
        var items = new List<WechatCallbackMeetingMediumUploadItem>();
        if (node == null)
            return items;

        List<PayloadNode>? members = null;
        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child != null && string.Equals(child.Name, node.Name, StringComparison.Ordinal))
            {
                members ??= new List<PayloadNode>();
                members.Add(child);
            }
        }

        if (members == null)
        {
            // 单元素形态：定位节点即 UploadInfo 元素本身（未触发合并投影）。
            AddMediumUploadItem(items, node);
            return items;
        }

        // 合并形态：同名容器的子节点即原始 UploadInfo 元素（保序）。
        for (var i = 0; i < members.Count; i++)
        {
            AddMediumUploadItem(items, members[i]);
        }

        return items;
    }

    private static void AddMediumUploadItem(List<WechatCallbackMeetingMediumUploadItem> items, PayloadNode node)
    {
        var item = new WechatCallbackMeetingMediumUploadItem();
        WechatCallbackMeetingMediumUploadItem.PayloadFieldMap.Bind(node, item);
        items.Add(item);
    }

    /// <summary>
    /// 平铺重复兄弟元素的分派选择器（OA 审批 <c>sys_approval_change</c> 子树的列表通用前置）。
    /// </summary>
    /// <param name="node">按 <c>[PayloadField]</c> 元素名定位到的节点。</param>
    /// <returns>
    /// 合并形态（≥2 个同名兄弟经投影归拢为同名容器）返回容器内的原始元素全集；
    /// 单元素形态（未触发合并，定位节点即元素本身）返回 <c>null</c>。
    /// </returns>
    /// <remarks>
    /// 判别依据：子节点中存在与容器<b>同名</b>的节点 ⇒ 本节点是合并投影产生的同名容器
    /// （合并语义保证容器与成员同名）；OA 审批子树的各列表（<c>SpRecord</c>/<c>Details</c>/
    /// <c>Notifyer</c>/<c>Comments</c>/<c>NodeList</c>/<c>SubNodeList</c>）均为此形态。
    /// </remarks>
    private static List<PayloadNode>? SelectCoalescedMembers(PayloadNode node)
    {
        List<PayloadNode>? members = null;
        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child != null && string.Equals(child.Name, node.Name, StringComparison.Ordinal))
            {
                members ??= new List<PayloadNode>();
                members.Add(child);
            }
        }

        return members;
    }

    /// <summary>OA 审批流程信息列表（官方 <c>SpRecord</c>，ApprovalInfo 下重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaApprovalRecord> RepeatOaApprovalRecords(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaApprovalRecord>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaApprovalRecord(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaApprovalRecord(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaApprovalRecord BindOaApprovalRecord(PayloadNode node)
    {
        var item = new WechatCallbackOaApprovalRecord();
        WechatCallbackOaApprovalRecord.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>OA 审批节点分支列表（官方 <c>Details</c>，SpRecord 内重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaApprovalDetail> RepeatOaApprovalDetails(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaApprovalDetail>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaApprovalDetail(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaApprovalDetail(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaApprovalDetail BindOaApprovalDetail(PayloadNode node)
    {
        var item = new WechatCallbackOaApprovalDetail();
        WechatCallbackOaApprovalDetail.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>OA 审批抄送人列表（官方 <c>Notifyer</c>，ApprovalInfo 下重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaApprovalNotifyer> RepeatOaApprovalNotifyers(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaApprovalNotifyer>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaApprovalNotifyer(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaApprovalNotifyer(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaApprovalNotifyer BindOaApprovalNotifyer(PayloadNode node)
    {
        var item = new WechatCallbackOaApprovalNotifyer();
        WechatCallbackOaApprovalNotifyer.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>OA 审批备注列表（官方 <c>Comments</c>，ApprovalInfo 下重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaApprovalComment> RepeatOaApprovalComments(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaApprovalComment>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaApprovalComment(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaApprovalComment(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaApprovalComment BindOaApprovalComment(PayloadNode node)
    {
        var item = new WechatCallbackOaApprovalComment();
        WechatCallbackOaApprovalComment.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>OA 审批流程节点列表（官方 <c>ProcessList/NodeList</c>，ProcessList 内重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaProcessNode> RepeatOaProcessNodes(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaProcessNode>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaProcessNode(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaProcessNode(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaProcessNode BindOaProcessNode(PayloadNode node)
    {
        var item = new WechatCallbackOaProcessNode();
        WechatCallbackOaProcessNode.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>OA 审批子节点列表（官方 <c>SubNodeList</c>，NodeList 内重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    public static List<WechatCallbackOaProcessSubNode> RepeatOaProcessSubNodes(PayloadNode? node)
    {
        var items = new List<WechatCallbackOaProcessSubNode>();
        if (node == null)
            return items;

        var members = SelectCoalescedMembers(node);
        if (members == null)
        {
            items.Add(BindOaProcessSubNode(node));
            return items;
        }

        for (var i = 0; i < members.Count; i++)
        {
            items.Add(BindOaProcessSubNode(members[i]));
        }

        return items;
    }

    private static WechatCallbackOaProcessSubNode BindOaProcessSubNode(PayloadNode node)
    {
        var item = new WechatCallbackOaProcessSubNode();
        WechatCallbackOaProcessSubNode.PayloadFieldMap.Bind(node, item);
        return item;
    }

    /// <summary>
    /// 带属性的嵌套项 → 列表（成员扩展属性 <c>&lt;ExtAttr&gt;&lt;Item Name Type&gt;&lt;Text/&gt;&lt;/Item&gt;&lt;/ExtAttr&gt;</c> 形态）；
    /// 节点缺失 ⇒ 空列表。
    /// </summary>
    /// <typeparam name="T">项类型，须有无参构造（上游签名要求 <c>where T : new()</c>）。</typeparam>
    /// <param name="node">容器节点。</param>
    /// <param name="itemName">项元素名。</param>
    /// <param name="nameAttribute">承载名称的属性名（官方固定 <c>Name</c>）。</param>
    /// <param name="valueElement">承载值的子元素名（官方固定 <c>Text</c>）。</param>
    public static List<T> ItemsWithAttributes<T>(
        PayloadNode? node, string itemName, string nameAttribute, string valueElement)
        where T : new()
    {
        var items = new List<T>();
        if (node == null || string.IsNullOrEmpty(itemName))
            return items;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, itemName, StringComparison.Ordinal))
                continue;

            var item = new T();
            if (item is WechatCallbackExtAttrItem extAttr)
            {
                extAttr.Name = LookupAttribute(child, nameAttribute);
                extAttr.Type = LookupAttribute(child, "Type");
                extAttr.Value = child.Child(valueElement ?? string.Empty)?.Value;
            }

            items.Add(item);
        }

        return items;
    }

    /// <summary>官方性别值域（1 = 男性，2 = 女性）。</summary>
    public static WechatUserGender? ParseGender(string? text)
        => text == "1" ? WechatUserGender.Male
            : text == "2" ? WechatUserGender.Female
            : null;

    /// <summary>官方成员激活状态值域（1 已激活 / 2 已禁用 / 4 未激活 / 5 退出企业）。</summary>
    public static WechatUserStatus? ParseStatus(string? text)
        => text == "1" ? WechatUserStatus.Activated
            : text == "2" ? WechatUserStatus.Disabled
            : text == "4" ? WechatUserStatus.NotActivated
            : text == "5" ? WechatUserStatus.Quit
            : null;

    /// <summary>
    /// 小数标量（官方 <c>Latitude</c>/<c>Longitude</c>/<c>Precision</c> 与 <c>Location_X</c>/<c>Location_Y</c>
    /// 的 <c>23.104</c> 形态）；<see cref="Number{T}"/> 只解析整数，故单列本方法。非法 ⇒ <c>null</c>。
    /// </summary>
    public static double? ParseReal(PayloadNode? node)
    {
        var text = node?.Value;
        if (string.IsNullOrEmpty(text))
            return null;

        if (!double.TryParse(text!.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return null;

        return value;
    }

    // ——— 嵌套结构（上游 G-ADR-17：Object / ItemsObject，由生成器发射调用）———

    /// <summary>
    /// 单对象嵌套（官方 <c>ScanCodeInfo</c>/<c>SendPicsInfo</c>/<c>SendLocationInfo</c> 形态）；
    /// 节点缺失 ⇒ <c>null</c>。内层字段由 <paramref name="accessor"/>（内层类型生成的映射表）递归 <c>Bind</c>。
    /// </summary>
    /// <typeparam name="TSingle">内层 DTO，须标注 <c>[PayloadContract]</c>（生成器只引用、不验证，G-ADR-17b）。</typeparam>
    /// <param name="node">嵌套对象节点（由 <c>[PayloadField]</c> 定位）。</param>
    /// <param name="accessor">内层映射表的非泛型桥（<see cref="IPayloadContractAccessor"/>）。</param>
    public static TSingle? Object<TSingle>(PayloadNode? node, IPayloadContractAccessor accessor)
        where TSingle : class
    {
        if (node == null || accessor == null)
            return null;

        if (accessor.CreateInstance() is not TSingle instance)
            return null;

        return (TSingle)accessor.Bind(node, instance);
    }

    /// <summary>
    /// 契约化对象项 → 列表（官方 <c>ApprovalNodes/ApprovalNode</c>、<c>SelectedItems/SelectedItem</c> 形态）；
    /// 节点缺失或不含 <paramref name="itemName"/> 项 ⇒ 空列表（语义不变量：绝不抛异常）。
    /// </summary>
    /// <typeparam name="TItem">项 DTO，须标注 <c>[PayloadContract]</c>（生成器只引用、不验证，G-ADR-17b）。</typeparam>
    /// <param name="node">容器节点（由 <c>[PayloadField]</c> 定位到容器本身）。</param>
    /// <param name="itemName">项元素名（<c>ItemName</c> 声明）。</param>
    /// <param name="itemAccessor">项映射表的非泛型桥（<see cref="IPayloadContractAccessor"/>）。</param>
    public static List<TItem> ItemsObject<TItem>(PayloadNode? node, string itemName, IPayloadContractAccessor itemAccessor)
        where TItem : class
    {
        var items = new List<TItem>();
        if (node == null || string.IsNullOrEmpty(itemName) || itemAccessor == null)
            return items;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, itemName, StringComparison.Ordinal))
                continue;

            if (itemAccessor.CreateInstance() is not TItem instance)
                continue;

            items.Add((TItem)itemAccessor.Bind(child, instance));
        }

        return items;
    }

    private static string? LookupAttribute(PayloadNode node, string? attributeName)
    {
        if (string.IsNullOrEmpty(attributeName))
            return null;

        return node.Attributes.TryGetValue(attributeName!, out var value) ? value : null;
    }
}
