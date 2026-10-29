// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>查询评价列表的过滤类型（<c>filterType</c>，官方枚举）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getccommentlist.html"/>。
/// 星级口径为官方原文「每 100 分对应 1 星」，故「差评」= 1~2 星、「好评」= 4~5 星、中评 = 3 星。
/// </remarks>
public static class WxaCommentFilterType
{
    /// <summary>全部差评：所有评价分数为 1 星、2 星的评价（<c>1</c>）。</summary>
    public const int AllNegative = 1;

    /// <summary>全部好评：所有评价分数为 4 星、5 星的评价（<c>2</c>）。</summary>
    public const int AllPositive = 2;

    /// <summary>差评待处理：所有未提交「和解挽回」的评价（<c>3</c>）。</summary>
    public const int NegativePending = 3;

    /// <summary>开发者待回复：所有待开发者回复的评价（<c>4</c>）。</summary>
    public const int ReplyPending = 4;

    /// <summary>差评已改评（<c>5</c>）。</summary>
    public const int NegativeRevised = 5;

    /// <summary>全部评价：所有好评、差评、中评（3 星）评价（<c>6</c>）。</summary>
    public const int All = 6;
}
