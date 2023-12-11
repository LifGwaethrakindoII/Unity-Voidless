﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
/// <summary>Event invoked when a Pool Object is beong deactivated.</summary>
/// <param name="_poolObject">Pool Object that has been deactivated.</param>
public delegate void OnPoolObjectDeactivation(IPoolObject _poolObject);

public interface IPoolObject
{
	event OnPoolObjectDeactivation onPoolObjectDeactivation;

	/// <summary>Is this Pool Object going to be destroyed when changing scene? [By default it destroys it].</summary>
	bool dontDestroyOnLoad { get; set; }
	/// <summary>Is this Pool Object active [preferibaly unavailable to recycle]?.</summary>
	bool active { get; set; }

	/// <summary>Callback invoked when this Pool Object is being created.</summary>
	void OnObjectCreation();

	/// <summary>Callback invoked when this Pool Object is being reseted.</summary>
	void OnObjectReset();

	/// <summary>Callback invoked when the object is deactivated.</summary>
	void OnObjectDeactivation();

	/// <summary>Callback invoked when this Pool Object is being destroyed.</summary>
	void OnObjectDestruction();
}
}