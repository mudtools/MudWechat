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

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 企业微信回调载荷转换器。
/// </summary>
/// <remarks>
/// <para>
/// <b>签名契约为硬约束</b>：方法必须 <c>static</c>、非泛型（除自身类型参数外）、参数形态固定 ——
/// 由上游 <c>PayloadFieldMapGenerator</c> 在编译期校验，不符即 <c>PAYLOAD004</c>。
/// 本类的方法集与上游推断表（<c>Text</c>/<c>Number&lt;T&gt;</c>/<c>Flag&lt;T&gt;</c>/<c>Delimited&lt;T&gt;</c>/
/// <c>Items&lt;T&gt;</c>/<c>ItemsWithAttributes&lt;T&gt;</c>）逐项对应。
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

    // ——— 二级/多级嵌套结构（超出上游「容器 → 单层同构项」表达力，按指南 §2.4 以 Method 逃生舱组装） ———

    /// <summary>
    /// 扫码信息（官方 <c>ScanCodeInfo/ScanType</c> + <c>ScanCodeInfo/ScanResult</c>）；节点缺失 ⇒ <c>null</c>。
    /// </summary>
    public static WechatCallbackScanCodeInfo? ParseScanCodeInfo(PayloadNode? node)
        => node == null
            ? null
            : new WechatCallbackScanCodeInfo
            {
                ScanType = Text(node.Child("ScanType")),
                ScanResult = Text(node.Child("ScanResult")),
            };

    /// <summary>
    /// 图片信息（官方 <c>SendPicsInfo/Count</c> + <c>SendPicsInfo/PicList/item/PicMd5Sum</c>）；
    /// 节点缺失 ⇒ <c>null</c>，缺失子项跳过（语义不变量：绝不抛异常）。
    /// </summary>
    public static WechatCallbackSendPicsInfo? ParseSendPicsInfo(PayloadNode? node)
    {
        if (node == null)
            return null;

        var info = new WechatCallbackSendPicsInfo
        {
            Count = Number<long>(node.Child("Count")),
        };

        var picList = node.Child("PicList");
        if (picList == null)
            return info;

        var children = picList.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var item = children[i];
            if (item == null || !string.Equals(item.Name, "item", StringComparison.Ordinal))
                continue;

            var md5 = Text(item.Child("PicMd5Sum"));
            if (md5 != null)
                info.PicMd5Sums.Add(md5);
        }

        return info;
    }

    /// <summary>
    /// 位置信息（官方 <c>SendLocationInfo/Location_X</c> 等 5 个子节点，坐标为小数）；
    /// 节点缺失 ⇒ <c>null</c>。
    /// </summary>
    public static WechatCallbackSendLocationInfo? ParseSendLocationInfo(PayloadNode? node)
        => node == null
            ? null
            : new WechatCallbackSendLocationInfo
            {
                LocationX = ParseReal(node.Child("Location_X")),
                LocationY = ParseReal(node.Child("Location_Y")),
                Scale = Number<long>(node.Child("Scale")),
                Label = Text(node.Child("Label")),
                Poiname = Text(node.Child("Poiname")),
            };

    /// <summary>
    /// 审批流程节点列表（官方 <c>ApprovalNodes/ApprovalNode</c>，节点内含 <c>Items/Item</c> 分支列表）；
    /// 节点缺失 ⇒ 空列表。
    /// </summary>
    public static List<WechatCallbackApprovalNode> ParseApprovalNodes(PayloadNode? node)
    {
        var nodes = new List<WechatCallbackApprovalNode>();
        if (node == null)
            return nodes;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, "ApprovalNode", StringComparison.Ordinal))
                continue;

            nodes.Add(new WechatCallbackApprovalNode
            {
                NodeStatus = Number<long>(child.Child("NodeStatus")),
                NodeAttr = Number<long>(child.Child("NodeAttr")),
                NodeType = Number<long>(child.Child("NodeType")),
                Items = ParseApprovalItems(child.Child("Items")),
            });
        }

        return nodes;
    }

    /// <summary>
    /// 审批分支列表（官方 <c>Items/Item</c>）；容器缺失或不含 <c>Item</c> ⇒ 空列表。
    /// </summary>
    private static List<WechatCallbackApprovalItem> ParseApprovalItems(PayloadNode? node)
    {
        var items = new List<WechatCallbackApprovalItem>();
        if (node == null)
            return items;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, "Item", StringComparison.Ordinal))
                continue;

            items.Add(new WechatCallbackApprovalItem
            {
                ItemName = Text(child.Child("ItemName")),
                ItemUserId = Text(child.Child("ItemUserId")),
                ItemImage = Text(child.Child("ItemImage")),
                ItemStatus = Number<long>(child.Child("ItemStatus")),
                ItemSpeech = Text(child.Child("ItemSpeech")),
                ItemOpTime = Number<long>(child.Child("ItemOpTime")),
            });
        }

        return items;
    }

    /// <summary>
    /// 抄送人列表（官方 <c>NotifyNodes/NotifyNode</c>）；节点缺失 ⇒ 空列表。
    /// </summary>
    public static List<WechatCallbackApprovalNotifyNode> ParseNotifyNodes(PayloadNode? node)
    {
        var nodes = new List<WechatCallbackApprovalNotifyNode>();
        if (node == null)
            return nodes;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, "NotifyNode", StringComparison.Ordinal))
                continue;

            nodes.Add(new WechatCallbackApprovalNotifyNode
            {
                ItemName = Text(child.Child("ItemName")),
                ItemUserId = Text(child.Child("ItemUserId")),
                ItemImage = Text(child.Child("ItemImage")),
            });
        }

        return nodes;
    }

    /// <summary>
    /// 模板卡片选中项列表（官方 <c>SelectedItems/SelectedItem</c>，项内含 <c>OptionIds/OptionId</c>）；
    /// 节点缺失 ⇒ 空列表。
    /// </summary>
    public static List<WechatCallbackTemplateCardSelectedItem> ParseSelectedItems(PayloadNode? node)
    {
        var items = new List<WechatCallbackTemplateCardSelectedItem>();
        if (node == null)
            return items;

        var children = node.Children;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child == null || !string.Equals(child.Name, "SelectedItem", StringComparison.Ordinal))
                continue;

            var optionIds = new List<string>();
            var optionContainer = child.Child("OptionIds");
            if (optionContainer != null)
            {
                var optionChildren = optionContainer.Children;
                for (var j = 0; j < optionChildren.Count; j++)
                {
                    var option = optionChildren[j];
                    if (option == null || !string.Equals(option.Name, "OptionId", StringComparison.Ordinal))
                        continue;

                    var id = Text(option);
                    if (id != null)
                        optionIds.Add(id);
                }
            }

            items.Add(new WechatCallbackTemplateCardSelectedItem
            {
                QuestionKey = Text(child.Child("QuestionKey")),
                OptionIds = optionIds,
            });
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
