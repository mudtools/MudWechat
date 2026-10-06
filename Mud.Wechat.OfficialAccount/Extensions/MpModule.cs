// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 微信公众号 API 模块枚举（对齐企微 <c>WechatModule</c>）。
/// </summary>
/// <remarks>
/// 令牌签发客户端（<c>IMpAuthentication</c>）位于 Abstractions 的 Authentication 注册组，
/// 随令牌底座自动注册，不经本枚举。
/// </remarks>
public enum MpModule
{
    /// <summary>
    /// 基础接口（获取微信 API 服务器 IP + 获取微信推送服务器 IP + 网络通信检测 3 端点）。
    /// </summary>
    /// <remarks>
    /// 公众号无「自建 / 套件 / 代开发」三类形态 ⇒ 单一接口直接作注册接口（不设应用类型子接口）。
    /// 官方对 3 个端点均支持第三方平台令牌（<c>component_access_token</c> /
    /// <c>authorizer_access_token</c>），<b>M0 仅覆盖自建形态</b>（AppId + AppSecret）。
    /// </remarks>
    Basic,

    /// <summary>
    /// 用户管理 → 标签管理（创建标签 / 获取标签 / 编辑标签 / 删除标签 / 获取标签下粉丝列表 /
    /// 批量打标签 / 批量取消标签 / 获取用户标签列表共 8 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：全部端点适用范围均为「公众号 / 服务号 —— 仅认证」（仅企业主体已认证账号可调用）；
    /// 标签上限 100 个、单用户标签上限 20 个、标签名 ≤ 30 字符。
    /// </remarks>
    Tag,

    /// <summary>
    /// 用户管理 → 用户信息（获取用户基本信息 / 批量获取用户基本信息 / 获取关注用户列表 / 设置用户备注名 /
    /// 获取黑名单列表 / 拉黑用户 / 取消拉黑用户共 7 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：均「仅认证」；<c>updateRemark</c> 正文原文为「暂时开放给微信认证的服务号」；
    /// 批量 100 条 / 关注者单批 10000 / 黑名单单批 1000 / 拉黑单次 20 个。
    /// </remarks>
    User,

    /// <summary>
    /// 令牌签发（<c>getAccessToken</c> + <c>getStableAccessToken</c>）：随 <c>AddMpApp</c> 自动注册，
    /// 本枚举成员仅供模块清单对齐使用（不作为 <c>AddAuthenticationApi()</c> 的必要入口）。
    /// </summary>
    Authentication,
}
