// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 回调事件信封的<b>最小公共字段契约</b>（企业微信 / 公众号共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何是接口而不是基类</b>：叶层无任何实现可共享（各产品线信封的字段集合、事件键解析规则、
/// 专有字段与便捷判定属性完全不同），而基类会迫使产品线信封继承并暴露叶层字段——
/// 既无法表达「公众号无 AgentID/InfoType」这类差异，也会让宿主处理器签名被迫降级到基类。
/// 接口只声明「两家都有的字段」，产品线信封各自实现（属性已存在时零改动）。
/// </para>
/// <para>
/// 本接口是**纯字段契约**：不引入 XML 类型（各产品线接收器负责「XML → 信封」的解析），
/// 也不引入任何产品线语义（守卫 CB-L1d 锁定）。
/// </para>
/// </remarks>
public interface IWechatCallbackEnvelope
{
    /// <summary>接收方标识（企业微信 ToUserName 节点：CorpId/SuiteId；公众号为目标公众号的原始 ID）。</summary>
    string? ToUserName { get; }

    /// <summary>发送方标识（企业微信 FromUserName 节点；公众号为用户 OpenID）。</summary>
    string? FromUserName { get; }

    /// <summary>消息创建时间（CreateTime 节点，Unix 秒字符串）。</summary>
    string? CreateTime { get; }

    /// <summary>消息类型（MsgType 节点；事件回调为 <c>event</c>）。</summary>
    string? MsgType { get; }

    /// <summary>事件类型（Event 节点；普通消息为空）。</summary>
    string? Event { get; }

    /// <summary>
    /// 解密后的原始明文（企微为 XML；公众号按模式为 XML 明文或密文解密结果）。
    /// </summary>
    /// <remarks><b>敏感载体</b>：可能包含凭据节点，不得整体写入日志/遥测/异常消息。</remarks>
    string? DecryptedXml { get; }

    /// <summary>时间戳（URL 查询参数，验签用）。</summary>
    string? TimeStamp { get; }

    /// <summary>随机数（URL 查询参数，验签用）。</summary>
    string? Nonce { get; }

    /// <summary>事件归属应用键（路由路径段，未命中时为 <c>null</c>）。</summary>
    string? AppKey { get; }

    /// <summary>事件类型键（处理器匹配键；解析规则由各产品线信封定义）。</summary>
    string EventTypeKey { get; }
}
