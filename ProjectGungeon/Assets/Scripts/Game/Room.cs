using UnityEngine;
using QFramework;
using System.Collections.Generic;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.ProjectGungeon
{
	public partial class Room : ViewController
	{
		List<Vector3> enemyPositions = new List<Vector3>();
		public void AddEnemyPosition(Vector3 enemyPosition)
		{
			enemyPositions.Add(enemyPosition);
		}
	}
}
