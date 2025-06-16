// generateTime 2025-06-16
// https://github.com/iohao/ioGame
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Protobuf;
using IoGame.Sdk;

namespace IoGame.Gen
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>Author: https://github.com/iohao/ioGame</remarks>
  public static class LoginAction
  {
    private static readonly int loginVerify_1_1 = CmdKit.MappingRequest(65537, "");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loginVerify"></param>
    /// <param name="callback"><see cref="Pb.Common.UserInfo"/> </param>
    /// <returns>RequestCommand</returns>
    /// <code>
    /// // 
    /// Pb.Common.LoginVerify loginVerify = ...;
    /// LoginAction.ofLoginVerify(loginVerify, result => {
    ///     // ioGame: your biz code.
    ///     // 
    ///     var value = result.GetValue&lt;Pb.Common.UserInfo&gt;();
    ///     result.Log(value);
    /// });
    /// </code>
    public static RequestCommand OfLoginVerify(Pb.Common.LoginVerify loginVerify, CallbackDelegate callback)
    {
        return RequestCommand.Of(loginVerify_1_1, loginVerify).OnCallback(callback).Execute();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loginVerify"></param>
    /// <returns>ResponseResult，<see cref="Pb.Common.UserInfo"/> </returns>
    /// <code>
    /// // 
    /// Pb.Common.LoginVerify loginVerify = ...;
    /// var result = await LoginAction.ofAwaitLoginVerify(loginVerify);
    /// // ioGame: your biz code.
    /// // 
    /// var value = result.GetValue&lt;Pb.Common.UserInfo&gt;();
    /// result.Log(value);
    /// </code>
    public static async Task<ResponseResult> OfAwaitLoginVerify(Pb.Common.LoginVerify loginVerify)
    {
        return await RequestCommand.OfAwait(loginVerify_1_1, loginVerify);
    }

  }
}