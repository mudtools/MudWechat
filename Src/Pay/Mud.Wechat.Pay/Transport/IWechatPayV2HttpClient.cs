// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>
/// APIv2 普通端点（<c>/pay/*</c>，无需证书）专用客户端契约 ——
/// <c>[HttpClientApi(HttpClient = <see cref="WechatPayV2HttpClientNames.TypeName"/>)]</c> 的注入目标。
/// </summary>
/// <remarks>
/// 标记式接口（成员全部来自 <see cref="IEnhancedHttpClient"/>），存在的意义是占住一个独立 DI 类型键，
/// 使本线拿到带 <see cref="WechatPayV2SigningHandler"/> 与 XML 序列化器的客户端，
/// 不与 APIv3 或其余产品线共享默认实例。
/// </remarks>
public interface IWechatPayV2HttpClient : IEnhancedHttpClient;
