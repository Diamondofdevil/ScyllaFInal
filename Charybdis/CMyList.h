/** @file CMyList.h ver 2009.09.09
*	Lista doblemente enlazada, modificada de la estructura común por eficiencia
*
*   Esta clase hace parte de la futura Colección de Estructuras de datos de
*   ColombiaUnderground team http://www.colombiaunderground.org
*   Está prohibida la copia de cualquiera de los archivos, o de partes de los archivos sin
*   hacer referencia a los autores.
*
*	Author: Iker@cuteam.org
*	Revisado y Documentado: Flacman@cuteam.org && iker@cuteam.org
*/

#ifndef __CMYLIST_H__
#define __CMYLIST_H__


#ifndef _CTIME_
#include <ctime>
#endif

/**
*	@brief Lista doblemente enlazada para uso generico. modificada de la estructura comun
*	por eficiencia y sobrecara de operadores para usabilidad.
*/
template<class TYPE>
class CMyList{
protected:
	/**	@brief Estructura nodo base */
	struct stListNode
	{
		/**	Argumento de nodo */
		TYPE Arg;
		/**	Nodo anterior */
		stListNode *pBfr;
		/** Nodo siguiente */
		stListNode *pNext;
	};

	/**	Contador de Items */
	long m_lCount;
	/**	Nodo actual, el nodo al que se está apuntando actualmente */
	stListNode *m_pCurrNode;
	/** Nodo raiz. Inicio de la estructura */
	stListNode *m_pRoot;
	/** Ultimo nodo de la estructura */
	stListNode *m_pLast;
	/** 
	*	Crea un nodo en la lista con contenido Arg entre los nodos Ant y Sig
	*	@param Arg as TYPE
	*	@param Ant as ListNode*
	*	@param Sig as ListNode*
	*	@return Retorna el nodo recien creado
	*/
	stListNode *CreateNode( TYPE, stListNode *, stListNode *);
	/**	
	*	Elimina el nodo especificado en Del
	*	@param Del as ListNode*
	*	@return Retorna El nodo anterior al creado, si no hay anterior devuelve Root
	*/
	stListNode *DeleteNode( stListNode* );
	/**
	*	Busca el nodo en la posicion Pos, lo elimina si Del es true
	*	@param Del as bool
	*	@param Pos as long
	*	@return Retorna el nodo buscado o true en caso que bDel sea true
	*/
	stListNode *pFindNode( bool, long);
	/*
	*	Devuelve el estado de la estructura y sus elementos a nulo
	*/
	void ResetAll( void );
public:
	/**
	*	Obtiene el tamaño del tipo del template
	*	@return Retorna el tamaño de TYPE.
	*/
	int GetSizeOfType( void );
	/**
	*	Añade un nodo nuevo al final de la lista
	*	@param Arg as TYPE
	*	@return Retorna Informacion del nodo anadido( Arg )
	*/
	TYPE AddNode( TYPE );
	/**
	*	Elimina el nodo de la posicion especificada por Pos
	*	@param Pos as long
	*	@return Retorna true si hay un nodo en esa posicion, false de lo contrario.
	*/
	bool RemoveNodeByPos( long );
	/**
	*	Resetea el apuntador al actual nodo a null
	*/
	void ResetCurrent( void );
	/**
	*	Setea el apuntador al nodo actual al indicado en la posicion especificada por Pos
	*	@see AcNode
	*	@param Pos as long
	*	@return Retorna true si hay un nodo en esa posicion, false de lo contrario
	*/
	bool SetCurrentByPos( long );
	/**
	*	Elimina todos los nodos de la estructura
	*/
	void RemoveAll( void );
	/**
	*	Encuentra el nodo en la posicion Pos y retorna la informacion del nodo
	*	@param Pos as long
	*	@return Retorna la informacion del nodo en la posicion Pos
	*/	
	TYPE FindNodeByPos( long );
	/**
	*	Devuelve la informacion del nodo siguiente al nodo actual
	*	@see AcNode
	*	@return Retorna la informacion del nodo siguiente
	*/
	TYPE GetNext( void );
	/**
	*	Devuelve la informacion del nodo anterior al nodo actual.
	*	@see AcNode
	*	@return Retorna la informacion del nodo anterior
	*/
	TYPE GetPrev( void );
	/**
	*	Devuelve la informacion del nodo Raiz
	*	@see Root
	*	@return Retorna la informacion del nodo Raiz
	*/
	TYPE GetRoot( void );
	/**
	*	Devuelve la informacion del nodo Last
	*	@see Last
	*	@return Retorna la informacion del ultimo nodo
	*/
	TYPE GetLast( void );
	/**
	*	Devuelve la informacion del nodo actual
	*	@see AcNode
	*	@return Retorna la informacion del nodo actual
	*/
	TYPE GetCurrentNode( void );
	/**
	*	Verifica si la estructura está vacia
	*	@return true si la lista está vacia, false de lo contrario
	*/
	bool IsEmpty( void );
	/**
	*	Retorna el número de nodos en la estructura
	*	@return Retorna el número de nodos en la estructura
	*/
	long GetSize( void );
	/**	
	*	Consigue la información de un nodo cualquiera (aleatorio) de la estructura y retorna el
	*	index del mismo en Val.
	*	@param Val as long *
	*	@return Retorna la informacion del nodo
	*/
	TYPE Randomize( long * );
	/**
	*	Inserta un nodo despues del nodo en la posicion especificada por Pos
	*	@param Pos as long
	*	@param Arg as TYPE
	*	@return Retorna la Informacion del nuevo nodo a añadir
	*/
	TYPE InsertAfter( long, TYPE );
	/**
	*	Inserta un nodo antes del nodo en la posicion especificada por Pos
	*	@param Pos as long
	*	@param Arg as TYPE
	*	@return Retorna la Informacion del nuevo nodo a anadir.
	*/
	TYPE InsertBefore( long, TYPE );
	/**
	*	Copia los elementos de la clase especificada en cmsSrc, el tipo de datos ha de tener el
	*	mismo tamaño
	*	@param cmsSrc as CMyList<TYPE>& cmsSrc
	*	@return Retorna la actual clase referenciada.
	*/
	CMyList<TYPE>& operator=( CMyList<TYPE>& );
	/**
	*	Devuelve la informacion del nodo actual y mueve el puntero al siguiente nodo
	*	@see GetNext
	*	@return Retorna la informacion del nodo actual y muve el puntero al siguiente nodo
	*/
	TYPE operator++();
	/**
	*	Mueve el puntero al siguiente nodo y devuelve la informacion del nodo
	*	@see GetNext
	*	@return Retorna la informacion del nodo siguiente
	*/
	TYPE operator++( int );
	/**
	*	Devuelve la informacion del nodo actual y mueve el puntero al siguiente nodo
	*	@see GetNext
	*	@return Retorna la informacion del nodo actual y muve el puntero al siguiente nodo
	*/
	TYPE operator--();
	/**
	*	Mueve el puntero al siguiente nodo y devuelve la informacion del nodo
	*	@see GetNext
	*	@return Retorna la informacion del nodo siguiente
	*/
	TYPE operator--( int );
	/**
	*	Encuentra el nodo en la posicion especificada en Pos y retorna la informacion del nodo
	*	@param Pos as long
	*	@return Retorna la informacion del nodo.
	*/	
	TYPE operator[] ( long );
	/**
	*	Constructor por defecto
	*/
	CMyList();
	/**
	*	Destructor por defecto, elimina todos los nodos de la lista y destruye el objeto
	*/
	~CMyList();
};

template<class TYPE>
CMyList<TYPE>::CMyList()
{
	m_pRoot		= NULL;
	m_pLast		= NULL;
	m_pCurrNode	= NULL;
	m_lCount	= 0;

	srand( (unsigned int)time( NULL ) );
}

template<class TYPE>
CMyList<TYPE>::~CMyList()
{
	RemoveAll();
}

template<class TYPE>
void CMyList<TYPE>::RemoveAll()
{
	if (IsEmpty() == false)
		while ( RemoveNodeByPos( 0 ) );

	ResetAll();
}

template<class TYPE>
void CMyList<TYPE>::ResetAll( void )
{
	m_pRoot		= NULL;
	m_pLast		= NULL;
	m_pCurrNode	= NULL;
	m_lCount	= 0;
}

template<class TYPE>
typename CMyList<TYPE>::stListNode*
CMyList<TYPE>::CreateNode( TYPE Arg, stListNode *pNext, stListNode *pBfr)
{
	stListNode *pRet = new stListNode;

	if (pRet == NULL)
		return NULL;

	pRet->Arg	= Arg;
	pRet->pBfr	= pBfr;
	pRet->pNext	= pNext;

	return pRet;
}

template<class TYPE>
int CMyList<TYPE>::GetSizeOfType( void )
{
	return sizeof( TYPE );
}

template<class TYPE>
TYPE CMyList<TYPE>::AddNode( TYPE Arg )
{
	if (m_pRoot == NULL) {
		m_pRoot = CreateNode( Arg, NULL, NULL);

		if (m_pRoot == NULL)
			return NULL;

		m_pLast = m_pRoot;

		++m_lCount;

		return m_pRoot->Arg;
	}

	m_pLast->pNext = CreateNode( Arg, NULL, m_pLast);

	if (m_pLast->pNext == NULL)
		return NULL;

	m_pLast = m_pLast->pNext;

	++m_lCount;

	return m_pLast->Arg;
}

template<class TYPE>
TYPE CMyList<TYPE>::GetNext( void )
{
	if (m_pCurrNode != NULL) {
		m_pCurrNode = m_pCurrNode->pNext;

		if (m_pCurrNode != NULL)
			return m_pCurrNode->Arg;

		return  NULL;
	}

	if (m_pRoot == NULL)
		return NULL;

	m_pCurrNode	= m_pRoot;

	return m_pCurrNode->Arg;
}

template<class TYPE>
TYPE CMyList<TYPE>::GetPrev( void )
{
	if (m_pCurrNode != NULL) {
		m_pCurrNode = m_pCurrNode->pBfr;

		if (m_pCurrNode != NULL)
			return m_pCurrNode->Arg;

		return NULL;
	}

	if (m_pLast == NULL)
		return NULL;

	m_pCurrNode = m_pLast;

	return m_pCurrNode->Arg;
}

template<class TYPE>
typename CMyList<TYPE>::stListNode*
CMyList<TYPE>::DeleteNode( stListNode *pDel )
{
	//Si el que quiere matar es root
	if (pDel == m_pRoot){
		stListNode *pAux = m_pRoot->pNext;
		//Liberar memoria
		delete m_pRoot;
		m_pRoot = NULL;
		//Si hay un siguiente, el siguiente es Root, sino, Root es nulo
		m_pRoot = pAux;
		if (m_pRoot != NULL) {
			m_pRoot->pBfr = NULL;
			--m_lCount;
		} else
			ResetAll();

		m_pCurrNode = NULL;
		return m_pRoot;
	}

	stListNode *pAuxNext	= pDel->pNext;
	stListNode *pAuxBfr		= pDel->pBfr;

	delete pDel;
	//Si siguiente AuxSig es NULL, qiuere decir que era el final de la lista
	//por lo tanto AuxAnt = al final
	pAuxBfr->pNext = pAuxNext;

	if (pAuxNext != NULL)
		pAuxNext->pBfr = pAuxBfr;
	else
		m_pLast = pAuxBfr;

	--m_lCount;

	m_pCurrNode	= NULL;

	return pAuxBfr;
}

template<class TYPE>
typename CMyList<TYPE>::stListNode*
CMyList<TYPE>::pFindNode( bool bDel, long lPos)
{
	register long lAt = 0;
	stListNode *pCurr;

	if (m_pRoot == NULL || lPos < 0 || lPos >= GetSize())
		return NULL;

	pCurr = m_pRoot;

	for ( ; lAt < lPos; ++lAt)
		pCurr = pCurr->pNext;
	
	if (bDel == true)
		return (stListNode *)DeleteNode( pCurr );

	return pCurr;
}

template<class TYPE>
TYPE CMyList<TYPE>::FindNodeByPos( long lPos )
{
	stListNode *pRet = NULL;
	
	if (lPos < 0 || lPos > GetSize())
		return NULL;
	
	pRet = pFindNode( false, lPos);

	if (pRet == NULL)
		return NULL;

	return pRet->Arg;
}

template<class TYPE>
bool CMyList<TYPE>::RemoveNodeByPos( long lPos )
{
	if (lPos < 0 || lPos > GetSize())
		return false;

	if (pFindNode( true, lPos) != NULL)
		return true;

	return false;
}

template<class TYPE>
TYPE CMyList<TYPE>::InsertBefore( long lPos, TYPE Arg )
{
	stListNode *pNode = NULL;
	stListNode *pAux, *pNew;

	if (lPos < 0 || lPos > GetSize())
		return NULL;

	pNode = pFindNode( false, lPos);

	if (pNode == NULL)
		return NULL;
	
	pNew = CreateNode( Arg, pNode, NULL);

	if (pNew == NULL)
		return NULL;

	pAux		= pNode->pBfr;
	pNew->pBfr	= pAux;

	++m_lCount;

	if (pAux == NULL) {
		pNode->pBfr	= pNew;
		pRoot		= pNew;
		return New->Arg;
	}

	pAux->pNext	= pNew;
	pNode->pBfr	= pNew;

	return pNew->Arg;
}

template<class TYPE>
TYPE CMyList<TYPE>::InsertAfter( long lPos, TYPE Arg )
{
	stListNode *pNode = NULL;
	stListNode *pAux, *pNew;

	if (lPos < 0 || lPos > GetSize())
		return NULL;

	pNode = pFindNode( false, lPos);

	if (pNode == NULL)
		return NULL;

	pNew = CreateNode( Arg, NULL, pNode);

	if (pNew == NULL)
		return NULL;

	pAux		= pNode->pNext;
	pNew->pNext	= pAux;

	++m_lCount;

	if (pAux == NULL) {
		pNode->pNext	= pNew;
		pLast			= pNew;
		return pNew->Arg;
	}

	pNode->pNext	= pNew;
	pAux->pBfr		= New;
	
	return pNew->Arg;
}

template<class TYPE>
TYPE CMyList<TYPE>::GetRoot( void )
{
	if (m_pRoot != NULL)
		return m_pRoot->Arg;

	return NULL;
}

template<class TYPE>
TYPE CMyList<TYPE>::GetLast( void )
{
	if (m_pLast != NULL)
		return m_pLast->Arg;

	return NULL;
}

template<class TYPE>
TYPE CMyList<TYPE>::GetCurrentNode( void )
{
	if (m_pCurrNode != NULL)
		return m_pCurrNode->Arg;

	return NULL;
}

template<class TYPE>
bool CMyList<TYPE>::IsEmpty( void )
{
	if (m_lCount == 0)
		return true;

	return false;
}

template<class TYPE>
void CMyList<TYPE>::ResetCurrent( void )
{
	m_pCurrNode = NULL;
}

template<class TYPE>
bool CMyList<TYPE>::SetCurrentByPos( long lPos )
{
	stListNode *pTmp = NULL;

	if (lPos < 0 || lPos > GetSize())
		return NULL;

	pTmp = pFindNode( false, lPos);

	if (pTmp == NULL)
		return false;

	m_pCurrNode = pTmp;

	return true;
}

template<class TYPE>
long CMyList<TYPE>::GetSize( void )
{
	return m_lCount;
}

template<class TYPE>
TYPE CMyList<TYPE>::Randomize( long *plVal )
{
	stListNode *pRet = NULL;
	
	if (plVal == NULL)
		return NULL;

	pRet = pFindNode( false, (*plVal = (rand( )%GetSize( ))));

	if (pRet == NULL) {
		*plVal = -1;
		return NULL;
	}

	return Ret->Arg;
}

template<class TYPE>
CMyList<TYPE>& CMyList<TYPE>::operator =(CMyList<TYPE>& cmsSrc )
{
	long lSize = 0, lCount = 0;

	if (GetSizeOfType() != cmsSrc.GetSizeOfType() || (lSize = cmsSrc.GetSize()) <= 0)
		return *this;

	RemoveAll();

	for ( ; lCount < lSize; ++lCount)
		AddNode( cmsSrc.FindNodeByPos( lCount ) );

	return *this;
}

template<class TYPE>
TYPE CMyList<TYPE>::operator ++()
{
	return GetNext();
}

template<class TYPE>
TYPE CMyList<TYPE>::operator ++( int )
{	
	TYPE tpTemp = GetCurrentNode();
	
	GetNext();

	return tpTemp;
}

template<class TYPE>
TYPE CMyList<TYPE>::operator --( int )
{	
	TYPE tpTemp = GetCurrentNode();
	
	GetPrev();

	return tpTemp;
}

template<class TYPE>
TYPE CMyList<TYPE>::operator --()
{
	return GetPrev();
}

template<class TYPE>
TYPE CMyList<TYPE>::operator []( long Pos )
{	
	return FindNodeByPos( Pos );
}

#endif