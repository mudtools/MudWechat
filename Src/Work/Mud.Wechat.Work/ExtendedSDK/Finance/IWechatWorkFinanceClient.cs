// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Finance;

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 企业微信会话内容存档 C SDK 客户端端口：拉取密文记录、解密单条记录、拉取媒体。
/// </summary>
/// <remarks>
/// <para>
/// <b>这不是 <c>[HttpClientApi]</c> 业务接口</b>：本域没有 HTTP 端点，全部动作都是进程内原生调用
/// ⇒ 不进 <c>WechatModule</c> 枚举、不进 <c>Add{域}Api()</c>、不参与令牌链路（守卫 FIN-B6）。
/// 注册入口是 <c>AddWechatFinanceSdk()</c>，实例经 <see cref="IWechatWorkFinanceClientFactory"/> 按机器人键取得。
/// </para>
/// <para>
/// <b>实例是按机器人的共享长生命周期对象</b>：官方允许同一 SDK 实例反复拉取，工厂按机器人键缓存实例；
/// 经工厂取得的实例<b>不要</b>自行 Dispose（工厂释放时统一销毁）。同一实例上的调用被串行化 ——
/// <c>seq</c> 游标协议本身要求逐次推进，并发拉取只会重复取回同一段记录。
/// </para>
/// <para>
/// <b>门面为 <c>...Async</c> 不代表存在真异步 I/O</b>：原生调用是阻塞的。异步只用于两处真实等待 ——
/// 取 RSA 私钥/secret 走 <c>ISecretProvider</c>（可能落在远端密钥系统），以及同游标退避重试的等待。
/// 调用方不应据此以为单机器人可以高并发。
/// </para>
/// </remarks>
public interface IWechatWorkFinanceClient
{
    /// <summary>本实例对应的机器人键（即配置里 <c>WechatFinance:Robots</c> 的字典键）。</summary>
    string RobotKey { get; }

    /// <summary>
    /// 按 <c>seq</c> 游标拉取一批<b>密文</b>会话记录。
    /// </summary>
    /// <param name="seq">起始游标：首次传 <c>0</c>，其后传上一次返回里<b>最大</b>的 <c>seq</c>。</param>
    /// <param name="limit">本次最多拉取条数（正整数）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>密文信封（<c>chatdata</c> 逐条含 <c>publickey_ver</c> / 两段密文）。</returns>
    /// <remarks>
    /// <para>
    /// <b>本方法不解密</b>，返回的每条记录须交给 <see cref="DecryptChatRecordAsync"/> 才能得到正文。
    /// </para>
    /// <para>
    /// <b>游标是「至少一次」语义</b>：以「上次最大 seq」续拉时，若同 seq 有多条记录，边界上的重复由宿主按
    /// <c>msgid</c> 幂等去重（官方未保证同 seq 唯一 ⇒ 不能假定不重复）。
    /// </para>
    /// <para>
    /// <b>业务限制（官方口径，待逐页核验）</b>：仅近 5 天数据 ⇒ 必须定期轮询；频率上限约 4000 次/分钟；
    /// 单次条数上限 1000。这些值<b>不</b>写成本地硬校验（未核验的上限会误拒合法调用），只在此处警示。
    /// </para>
    /// </remarks>
    Task<FinanceChatDataEnvelope> GetChatDataAsync(
        ulong seq, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解密一条会话记录（RSA 取会话密钥 → 原生 <c>DecryptData</c> → 明文 JSON）。
    /// </summary>
    /// <param name="row"><see cref="GetChatDataAsync"/> 返回的一行密文。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>解密后的明文消息（按 <c>msgtype</c> 取对应正文属性）。</returns>
    /// <remarks>
    /// <para>
    /// <b>版本号决定用哪把私钥</b>：取 <see cref="FinanceChatDataRow.PublicKeyVersion"/> 在
    /// <c>WechatFinanceRobotOptions.PrivateKeySecretNames</c> 里的密钥名，经 <c>ISecretProvider</c> 取私钥。
    /// 查无该版本即抛（不回落「最新一把」——拿错私钥的症状是后续 <c>DecryptData</c> 失败，归因成本极高）。
    /// </para>
    /// <para>
    /// <b>明文即敏感</b>：返回值承载会话正文，宿主不得整体写日志、遥测或异常消息（守卫 FIN-B5 同样约束 SDK 侧）。
    /// </para>
    /// </remarks>
    Task<FinanceChatMessage> DecryptChatRecordAsync(
        FinanceChatDataRow row, CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取媒体的<b>一个分片</b>（官方单片上限 512KB）。
    /// </summary>
    /// <param name="indexBuffer">续传游标：首片传 <c>null</c>，后续传上一片的
    /// <see cref="FinanceMediaChunk.NextIndexBuffer"/>。</param>
    /// <param name="sdkFileId">消息正文里的 <c>sdkfileid</c>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>本片结果（含是否已到末片）。</returns>
    /// <remarks>
    /// 逐片落盘的场景用本方法（不要把聚合结果先攒进内存再写文件）；需要完整字节请用
    /// <see cref="GetMediaDataAsync"/>。
    /// </remarks>
    Task<FinanceMediaChunk> GetMediaChunkAsync(
        string? indexBuffer, string sdkFileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取完整媒体文件（内部按游标续传、以完成标记收尾、逐片拼接）。
    /// </summary>
    /// <param name="sdkFileId">消息正文里的 <c>sdkfileid</c>。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>完整字节。</returns>
    /// <remarks>
    /// <para>
    /// <b>总量受 <c>byte[]</c> 极限约束</b>：超过 <see cref="int"/> 数组上限即抛错并引导改用
    /// <see cref="GetMediaChunkAsync"/> 逐片落盘 —— 边界由返回类型自然给出，不另设配置项。
    /// </para>
    /// <para>
    /// <b>取消不可中断已进入的原生调用</b>：令牌只在分片边界生效。
    /// </para>
    /// </remarks>
    Task<byte[]> GetMediaDataAsync(string sdkFileId, CancellationToken cancellationToken = default);
}
