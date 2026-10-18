// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools;

// 工具面取值闭集的声明处：策展声明面的参数以 [ToolParameter("x", "", EnumType = typeof(本文件某类型))] 引用。
//
// 为什么是「常量类 + const int」而不是 C# enum：官方码值是整数（SDK 请求 DTO 收 int/int?），
// 而模型侧契约是「按名字传值」。声明面参数类型保持 int/int?（与 SDK 一致，执行器零改动），
// 源生成器据此把 Schema 渲染为 {"type":"string","enum":[常量名…]}，并发射「常量名 → 官方整数」映射表，
// 由 ToolArgs.OptionalNamedInt / RequireNamedInt 在解包期还原成 SDK 需要的数值。
//
// 成员名即模型可见字面量：重命名成员等于改契约（golden 快照会漂移，需同批 git diff 复核）。

/// <summary>微文档 <c>doc_type</c> 取值闭集（官方开放 4 类）。</summary>
internal static class WechatWedocDocTypes
{
    /// <summary>文档（官方值 3）。</summary>
    public const int Document = 3;

    /// <summary>表格（官方值 4）。</summary>
    public const int Spreadsheet = 4;

    /// <summary>智能表格（官方值 10）。</summary>
    public const int SmartSheet = 10;

    /// <summary>智能文档（官方值 11）。</summary>
    public const int SmartDoc = 11;
}

/// <summary>打卡 <c>data_type</c> 取值闭集。</summary>
internal static class WechatCheckinDataTypes
{
    /// <summary>上下班打卡（官方值 1）。</summary>
    public const int Commute = 1;

    /// <summary>外出打卡（官方值 2）。</summary>
    public const int Field = 2;

    /// <summary>全部打卡（官方值 3，官方默认）。</summary>
    public const int All = 3;
}

/// <summary>客户群 <c>status_filter</c> 取值闭集（跟进状态）。</summary>
internal static class WechatGroupChatStatusFilters
{
    /// <summary>全部（官方值 0，官方默认）。</summary>
    public const int All = 0;

    /// <summary>离职待继承（官方值 1）。</summary>
    public const int PendingInherit = 1;

    /// <summary>离职继承中（官方值 2）。</summary>
    public const int Inheriting = 2;

    /// <summary>离职继承完成（官方值 3）。</summary>
    public const int Inherited = 3;
}

/// <summary>朋友圈 <c>filter_type</c> 取值闭集（发表来源）。</summary>
internal static class WechatMomentFilterTypes
{
    /// <summary>企业发表（官方值 0）。</summary>
    public const int CorpPublish = 0;

    /// <summary>个人发表（官方值 1）。</summary>
    public const int PersonalPublish = 1;

    /// <summary>全部（官方值 2，官方默认）。</summary>
    public const int All = 2;
}

/// <summary>消息发送 <c>safe</c> 取值闭集（保密模式）。</summary>
internal static class WechatMessageSafeModes
{
    /// <summary>可对外（官方值 0，官方默认）。</summary>
    public const int Public = 0;

    /// <summary>保密（官方值 1）。</summary>
    public const int Confidential = 1;
}

/// <summary>客户群统计 <c>order_by</c> 取值闭集（排行维度）。</summary>
internal static class WechatStatisticsOrderBy
{
    /// <summary>新增群数（官方值 1，官方默认）。</summary>
    public const int NewChatCount = 1;

    /// <summary>群总数（官方值 2）。</summary>
    public const int ChatTotal = 2;

    /// <summary>新增群人数（官方值 3）。</summary>
    public const int NewMemberCount = 3;

    /// <summary>群总人数（官方值 4）。</summary>
    public const int MemberTotal = 4;
}
