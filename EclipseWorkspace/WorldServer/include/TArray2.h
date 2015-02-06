#if !defined(TARRAY2_INCLUDED)
#define TARRAY2_INCLUDED

#include <cassert>
#include <algorithm>

template<typename T>
class TArray2
{
   public:

      // Typedefs
      typedef T value_type;
      typedef value_type* iterator;
      typedef const value_type* const_iterator;

      // Constructor
      TArray2(size_t rows=1, size_t cols=1)
      : m_Rows(0),
        m_Cols(0),
        m_OwnsData(true),
        p_Data(0)
      {
         resizeTo(rows,cols);
      }

      // Construct given external data
      TArray2(T* pAry, bool ownsData, size_t rows=1, size_t cols=1)
      : m_Rows(rows),
        m_Cols(cols),
        m_OwnsData(ownsData),
        p_Data(pAry)
      {
         assert(0 != p_Data);
      }

      // Copy constructor
      TArray2(const TArray2<T>& a)
      {
         resizeTo( a.rows(),a.cols() );
         std::copy( a.begin(), a.end(), p_Data);
         m_OwnsData = true;
      }

      // Assignment operator
      TArray2& operator = (const TArray2<T>& a)
      {
         resizeTo( a.rows(),a.cols() );
         std::copy( a.begin(), a.end(), p_Data);
         m_OwnsData = true;
         return *this;
      }

      // Destructor
      ~TArray2()
      {
         if (m_OwnsData) { delete[] p_Data; }
      }

      // Dimensions retrieval
      inline size_t rows() const { return m_Rows; }
      inline size_t cols() const { return m_Cols; }
      inline size_t size() const { return rows()*cols(); }
      inline size_t bytes() const { return bytes()*sizeof(value_type); }

//      // Fill with a value
//      void fill(value_type val)
//      {
//         std::uninitialized_fill_n( p_Data, size(), val );
//      }

      // Swap contents with another array
      void swap(TArray2<T>& ary)
      {
         std::swap( m_Rows, ary.m_Rows );
         std::swap( m_Cols, ary.m_Cols );
         std::swap( m_OwnsData, ary.m_OwnsData );
         std::swap( p_Data, ary.p_Data );
      }

      // Resize data bank
      void resizeTo(size_t rows, size_t cols)
      {
         // Not allowed for external data
         assert( m_OwnsData );

         // Sanity check
         size_t nElements = rows*cols;
         assert( nElements>0 );
         // If we have existing data
         if ( 0 != p_Data )
         {
            // No change, return
            if ( rows==m_Rows && cols==m_Cols )
            {
               return;
            }
            delete[] p_Data;
            p_Data = 0;
         }
         // Allocate data bank
         p_Data = new value_type[nElements];
         m_Rows = rows;
         m_Cols = cols;
      }

      // STL style iterators
      inline const_iterator begin() const { return p_Data; }
      inline const_iterator end() const { return p_Data + size(); }
      inline iterator begin() { return p_Data; }
      inline iterator end() { return p_Data + size(); }

      // Array indexing operators
      inline const T& operator () ( size_t i ) const { return p_Data[ checkedIndex(i) ]; }
      inline const T& operator () ( size_t i, size_t j ) const { return p_Data[ checkedIndex( i, j ) ]; }
      inline T& operator () ( size_t i ) { return p_Data[ checkedIndex(i) ]; }
      inline T& operator () ( size_t i, size_t j ) { return p_Data[ checkedIndex( i, j ) ]; }

      // Get pointers to internal data
      inline const T* c_data() const { return p_Data; }
      inline T* c_data() { return p_Data; }

   private:

      size_t checkedIndex(size_t indx) const
      {
         assert( indx < size() );
         return indx;
      }
      size_t checkedIndex(size_t iRow, size_t jCol) const
      {
         size_t k = m_Cols*iRow + jCol;
         assert( k < size() );
         return k;
      }

   private:

      size_t m_Rows;
      size_t m_Cols;
      bool m_OwnsData;
      T* p_Data;

}; // class TArray2

#endif // #if !defined(TARRAY2_INCLUDED)
