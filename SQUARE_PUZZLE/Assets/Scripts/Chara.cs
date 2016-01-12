using UnityEngine;
using System.Collections;

// キャラの情報
public class Chara : Datas {

	protected const int numOfBlueType = 3;			                    //type青のキャラ数
	protected const int numOfRedType = 3;			                    //type赤のキャラ数
	protected const int numOfYellowType = 3;		                    //type黄のキャラ数
	public Sprite[] bluetypeSprites = new Sprite[numOfBlueType];        //type青のキャラ画像
	public Sprite[] redtypeSprites = new Sprite[numOfRedType];          //type赤のキャラ画像
	public Sprite[] yellowtypeSprites = new Sprite[numOfYellowType];    //type黄のキャラ画像
	public int[][] scoreBuffs = new int[numOfType][];                   //scoreのバフ(インデックス:[type][キャラNo] )
	public int[][,] panelBuffs = new int[numOfType][,];                 //panelのバフ(インデックス:[type][キャラNo,パネルtype] )

	// Use this for initialization
	void Start () {
        // type青のscoreバフ
		scoreBuffs [0] = new int[numOfBlueType]{0,50,0};
        // type赤のscoreバフ
		scoreBuffs [1] = new int[numOfRedType]{0,25,0};
        // type黄のscoreバフ
		scoreBuffs [2] = new int[numOfYellowType]{0,20,0};
        // type青のscoreバフ
		panelBuffs[0] = new int[numOfBlueType,numOfType] {{0,0,0},{0,0,0},{50,0,0}};
        // type赤のscoreバフ
		panelBuffs[1] = new int[numOfRedType,numOfType] {{0,0,0},{0,25,0},{0,50,0}};
        // type黄のscoreバフ
		panelBuffs[2] = new int[numOfYellowType,numOfType]{{0,0,0},{10,10,10},{0,0,50}};
	}	
}
